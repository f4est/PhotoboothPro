using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;
using System.Diagnostics;
using NAudio.Wave;
using System.Windows.Controls.Primitives;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using System.Linq;

namespace UnifiedPhotoBooth
{
    public partial class VideoBoothPage : Page
    {
        private GoogleDriveService _driveService;
        private string _eventFolderId;
        private string _eventName;
        
        private VideoCapture _capture;
        private DispatcherTimer _previewTimer;
        private System.Threading.CancellationTokenSource _captureCts;
        private System.Threading.Tasks.Task _captureTask;
        
        private VideoWriter _videoWriter;
        private bool _isRecording;
        private string _recordingFilePath;
        private string _lastUniversalFolderId;
        
        private DispatcherTimer _countdownTimer;
        private int _countdownValue;
        
        private DispatcherTimer _recordingTimer;
        private TimeSpan _recordingTime;
        
        private string _audioFilePath;
        
        private bool _isPlaying;
        private WaveInEvent _audioCapture;
        private WaveFileWriter _audioWriter;
        
        private Action _onCountdownComplete;
        
        // Добавляем поле для хранения пути к временному видеофайлу
        private string _tempVideoPath;
        
        // Добавляем поле для отслеживания режима отображения (превью или видео)
        private bool _showingPreview = true;
        
        // Добавляем поля для контроля времени записи
        private int _framesRecorded;
        private double _targetFps;
        
        // Поля для точной записи видео
        private System.Threading.Timer _recordingFrameTimer; // Высокоточный таймер для записи
        private DateTime _recordingStartTime;
        private Mat _lastFrame; // Буфер для последнего кадра
        private readonly object _frameLock = new object(); // Блокировка для потокобезопасности
        
        public VideoBoothPage(GoogleDriveService driveService, string eventFolderId = null)
        {
            InitializeComponent();
            
            _driveService = driveService;
            _eventFolderId = eventFolderId;
            
            if (!string.IsNullOrEmpty(_eventFolderId))
            {
                _eventName = GetEventNameById(_eventFolderId);
                txtEventName.Text = _eventName;
            }
            
            // Инициализация таймера предпросмотра (больше не управляет захватом кадров)
            _previewTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
            _previewTimer.Tick += (s, e) => { /* зарезервировано */ };
            
            
            
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += CountdownTimer_Tick;
            
            _recordingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _recordingTimer.Tick += RecordingTimer_Tick;
            
            Loaded += VideoBoothPage_Loaded;
            Unloaded += VideoBoothPage_Unloaded;
            
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
        }
        
        private string GetEventNameById(string eventId)
        {
            try
            {
                var events = _driveService.ListEvents();
                foreach (var evt in events)
                {
                    if (evt.Value == eventId)
                    {
                        return evt.Key;
                    }
                }
            }
            catch { }
            
            return "Неизвестное событие";
        }
        
        private void VideoBoothPage_Loaded(object sender, RoutedEventArgs e)
        {
            StartPreview();
        }
        
        private void VideoBoothPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                StopRecording();
            }
            
            StopPreview();
            
            mediaPlayer.Stop();
            mediaPlayer.Source = null;
        }
        
        private void StartPreview()
        {
            try
            {
                _capture = new VideoCapture(SettingsWindow.AppSettings.CameraIndex);
                if (!_capture.IsOpened())
                {
                    ShowError("Не удалось открыть камеру. Проверьте настройки.");
                    return;
                }

                _captureCts = new System.Threading.CancellationTokenSource();
                _captureTask = System.Threading.Tasks.Task.Run(() => CaptureLoop(_captureCts.Token));

                ShowStatus("Готов к записи", "Нажмите 'Начать запись', чтобы записать видео");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при запуске предпросмотра: {ex.Message}");
            }
        }
        
        private void StopPreview()
        {
            _captureCts?.Cancel();
            try { _captureTask?.Wait(500); } catch {}
            _captureTask = null;

            _capture?.Dispose();
            _capture = null;
        }
        

        
        private void RecordingFrameTimer_Tick(object state)
        {
            if (!_isRecording || _videoWriter == null || !_videoWriter.IsOpened())
                return;
            
            try
            {
                lock (_frameLock)
                {
                    if (_lastFrame != null && !_lastFrame.Empty())
                    {
                        // Записываем кадр в видео
                        Mat frameForRecording = new Mat();
                        Cv2.Resize(_lastFrame, frameForRecording, _videoWriter.FrameSize);
                        _videoWriter.Write(frameForRecording);
                        frameForRecording.Dispose();
                        
                        _framesRecorded++;
                        
                        // Логируем прогресс каждые 30 кадров
                        if (_framesRecorded % 30 == 0)
                        {
                            var elapsed = DateTime.Now - _recordingStartTime;
                            var expectedFrames = (int)(elapsed.TotalSeconds * _targetFps);
                            System.Diagnostics.Debug.WriteLine($"Записано кадров: {_framesRecorded}, Ожидалось: {expectedFrames}, FPS: {_targetFps}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при записи кадра в видео: {ex.Message}");
            }
        }
        
        private void CaptureLoop(System.Threading.CancellationToken token)
        {
            // Определяем целевой FPS для предпросмотра и записи
            double cameraFps = SettingsWindow.AppSettings.VideoFps;
            if (cameraFps <= 0)
            {
                cameraFps = _capture.Get(OpenCvSharp.VideoCaptureProperties.Fps);
                if (cameraFps <= 0) cameraFps = 30.0;
            }

            var frameInterval = TimeSpan.FromMilliseconds(System.Math.Max(1, 1000.0 / cameraFps));

            while (!token.IsCancellationRequested && _capture != null && _capture.IsOpened())
            {
                try
                {
                    using (var frame = new Mat())
                    {
                        if (!_capture.Read(frame) || frame.Empty())
                        {
                            System.Threading.Thread.Sleep(1);
                            continue;
                        }

                        ApplyRotation(frame);
                        if (SettingsWindow.AppSettings.MirrorMode)
                        {
                            Cv2.Flip(frame, frame, FlipMode.Y);
                        }

                        Mat processed = AdjustAspectRatio(frame, 1200.0 / 1800.0);
                        ApplyOverlay(processed);

                        // Пишем кадр сразу при захвате — без потери кадров
                        if (_isRecording && _videoWriter != null && _videoWriter.IsOpened())
                        {
                            var recMat = new Mat();
                            Cv2.Resize(processed, recMat, _videoWriter.FrameSize);
                            _videoWriter.Write(recMat);
                            recMat.Dispose();
                            _framesRecorded++;
                        }

                        // Обновляем предпросмотр всегда (не ограничиваем частотой)
                        var bmp = BitmapSourceConverter.ToBitmapSource(processed);
                        // Важно: замораживаем BitmapSource, чтобы безопасно передать его в UI-поток
                        if (bmp.CanFreeze)
                        {
                            bmp.Freeze();
                        }
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            imgPreview.Source = bmp;
                        }));

                        if (processed != frame) processed.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка в CaptureLoop: {ex.Message}");
                }
            }
        }
        
        private void ApplyRotation(Mat frame)
        {
            OpenCvSharp.RotateFlags? rotateMode = null;
            
            switch (SettingsWindow.AppSettings.RotationMode)
            {
                case "90° вправо (вертикально)":
                    rotateMode = OpenCvSharp.RotateFlags.Rotate90Clockwise;
                    break;
                case "90° влево (вертикально)":
                    rotateMode = OpenCvSharp.RotateFlags.Rotate90Counterclockwise;
                    break;
                case "180°":
                    rotateMode = OpenCvSharp.RotateFlags.Rotate180;
                    break;
            }
            
            if (rotateMode.HasValue)
            {
                Cv2.Rotate(frame, frame, rotateMode.Value);
            }
        }
        
        private void ApplyOverlay(Mat frame)
        {
            string overlayPath = SettingsWindow.AppSettings.OverlayImagePath;
            if (string.IsNullOrEmpty(overlayPath) || !System.IO.File.Exists(overlayPath))
                return;
            
            try
            {
                using (var overlay = Cv2.ImRead(overlayPath, ImreadModes.Unchanged))
                {
                    if (overlay.Empty())
                        return;
                    
                    // Масштабируем оверлей под размер кадра
                    var resizedOverlay = new Mat();
                    Cv2.Resize(overlay, resizedOverlay, frame.Size());
                    
                    // Если у оверлея есть альфа-канал (прозрачность)
                    if (resizedOverlay.Channels() == 4)
                    {
                        // Используем первую перегрузку, которая возвращает массив Mat
                        Mat[] bgra = Cv2.Split(resizedOverlay);
                        
                        // Используем alpha канал
                        using (var alpha = bgra[3])
                        {
                            // Накладываем с учетом прозрачности
                            for (int i = 0; i < 3; i++)
                            {
                                using (var dst = new Mat())
                                {
                                    Cv2.BitwiseAnd(bgra[i], bgra[i], dst, alpha);
                                    using (var invAlpha = new Mat())
                                    {
                                        Cv2.BitwiseNot(alpha, invAlpha);
                                        using (var frameChannel = new Mat())
                                        {
                                            Cv2.ExtractChannel(frame, frameChannel, i);
                                            using (var dstFrame = new Mat())
                                            {
                                                Cv2.BitwiseAnd(frameChannel, frameChannel, dstFrame, invAlpha);
                                                Cv2.Add(dst, dstFrame, dst);
                                                Cv2.InsertChannel(dst, frame, i);
                                            }
                                        }
                                    }
                                }
                            }
                            
                            // Освобождаем ресурсы каналов
                            for (int i = 0; i < 4; i++)
                            {
                                if (i != 3) // alpha уже в using
                                {
                                    bgra[i]?.Dispose();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Если нет прозрачности, просто накладываем с прозрачностью
                        Cv2.AddWeighted(frame, 0.7, resizedOverlay, 0.3, 0, frame);
                    }
                    
                    // Освобождаем ресурсы
                    resizedOverlay?.Dispose();
                }
            }
            catch
            {
                // Игнорируем ошибки при наложении оверлея
            }
        }
        
        private void StartCountdown(int seconds, Action onComplete)
        {
            _countdownValue = seconds;
            txtCountdown.Text = _countdownValue.ToString();
            txtCountdown.Visibility = Visibility.Visible;
            
            // Устанавливаем делегат для обратного вызова по завершении отсчета
            _onCountdownComplete = onComplete;
            
            // Запускаем таймер обратного отсчета
            _countdownTimer.Start();
        }
        
        private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            _recordingTime = _recordingTime.Add(TimeSpan.FromSeconds(1));
            txtRecordingTime.Text = _recordingTime.ToString(@"mm\:ss");
            
            // Проверяем, отображается ли счетчик времени
            if (txtRecordingTime.Visibility != Visibility.Visible)
            {
                txtRecordingTime.Visibility = Visibility.Visible;
            }
            
            // Автоматическая остановка после достижения лимита времени
            if (_recordingTime.TotalSeconds >= SettingsWindow.AppSettings.RecordingDuration)
            {
                StopRecording();
            }
        }
        
        private void BtnStartRecording_Click(object sender, RoutedEventArgs e)
        {
            // Убираем проверку на выбор события
            // Скрываем плеер, если он был показан
            mediaPlayer.Stop();
            mediaPlayer.Visibility = Visibility.Collapsed;
            
            // Показываем превью
            imgPreview.Visibility = Visibility.Visible;
            
            // Скрываем QR-код, если он был показан
            imgQrCode.Visibility = Visibility.Collapsed;
            txtQrCodeUrl.Visibility = Visibility.Collapsed;
            
            // Обновляем интерфейс
            btnStartRecording.Visibility = Visibility.Collapsed;
            btnReset.Visibility = Visibility.Collapsed;
            btnShare.Visibility = Visibility.Collapsed;
            btnPlayPause.Visibility = Visibility.Collapsed;
            
            ShowStatus("Подготовка к записи", "Приготовьтесь...");
            
            // Запускаем отсчет перед записью
            StartCountdown(SettingsWindow.AppSettings.VideoCountdownTime, StartRecording);
        }
        
        private void StartRecording()
        {
            try
            {
                // Создаем директории для записи
                string recordingsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recordings");
                Directory.CreateDirectory(recordingsDir);
                
                // Генерируем имя файла на основе текущей даты и времени с миллисекундами для уникальности
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                string videoFileName = $"video_{timestamp}.mp4";
                string videoPath = Path.Combine(recordingsDir, videoFileName);
                
                // Сохраняем путь к видеофайлу
                _recordingFilePath = videoPath;
                
                // Инициализация _audioFilePath
                _audioFilePath = videoPath.Replace(".mp4", ".wav");
                
                // Получаем размеры кадра с учетом поворота
                int frameWidth = (int)_capture.Get(OpenCvSharp.VideoCaptureProperties.FrameWidth);
                int frameHeight = (int)_capture.Get(OpenCvSharp.VideoCaptureProperties.FrameHeight);
                
                // Применяем поворот, если необходимо
                string rotationMode = SettingsWindow.AppSettings.RotationMode;
                bool isRotated = rotationMode == "90° вправо (вертикально)" || rotationMode == "90° влево (вертикально)";
                
                if (isRotated)
                {
                    // При повороте на 90 градусов меняем местами ширину и высоту
                    int temp = frameWidth;
                    frameWidth = frameHeight;
                    frameHeight = temp;
                }
                
                // Убеждаемся, что размеры кратны 2 (требование многих кодеков)
                frameWidth = frameWidth - (frameWidth % 2);
                frameHeight = frameHeight - (frameHeight % 2);
                
                // Получаем частоту кадров из настроек или реальную частоту камеры
                double fps = SettingsWindow.AppSettings.VideoFps;
                if (fps <= 0)
                {
                    fps = _capture.Get(OpenCvSharp.VideoCaptureProperties.Fps);
                    if (fps <= 0)
                    {
                        fps = 30.0; // Значение по умолчанию
                    }
                }
                
                // Инициализируем VideoWriter с различными кодеками
                try
                {
                    // Получаем предпочтительный кодек из настроек
                    string preferredCodec = SettingsWindow.AppSettings.VideoCodec;
                    
                    // Список кодеков для попытки (в порядке предпочтения)
                    var allCodecs = new[]
                    {
                        new { FourCC = VideoWriter.FourCC('H', '2', '6', '4'), Extension = ".mp4", Name = "H.264", Tag = "H264" },
                        new { FourCC = VideoWriter.FourCC('X', '2', '6', '4'), Extension = ".mp4", Name = "X264", Tag = "X264" },
                        new { FourCC = VideoWriter.FourCC('m', 'p', '4', 'v'), Extension = ".mp4", Name = "MP4V", Tag = "MP4V" },
                        new { FourCC = VideoWriter.FourCC('M', 'J', 'P', 'G'), Extension = ".avi", Name = "MJPG", Tag = "MJPG" },
                        new { FourCC = VideoWriter.FourCC('X', 'V', 'I', 'D'), Extension = ".avi", Name = "XVID", Tag = "XVID" },
                        new { FourCC = VideoWriter.FourCC('D', 'I', 'V', 'X'), Extension = ".avi", Name = "DIVX", Tag = "DIVX" }
                    };
                    
                    // Если выбран конкретный кодек, ставим его первым
                    var codecAttempts = allCodecs.ToList();
                    if (preferredCodec != "Auto")
                    {
                        var preferred = codecAttempts.FirstOrDefault(c => c.Tag == preferredCodec);
                        if (preferred != null)
                        {
                            codecAttempts.Remove(preferred);
                            codecAttempts.Insert(0, preferred);
                        }
                    }
                    
                    bool videoWriterOpened = false;
                    string finalVideoPath = videoPath;
                    
                    foreach (var codec in codecAttempts)
                    {
                        try
                        {
                            _videoWriter?.Dispose();
                            _videoWriter = null;
                            
                            // Изменяем расширение файла в зависимости от кодека
                            if (codec.Extension != ".mp4")
                            {
                                finalVideoPath = videoPath.Replace(".mp4", codec.Extension);
                            }
                            
                            _videoWriter = new VideoWriter(finalVideoPath, codec.FourCC, fps, new OpenCvSharp.Size(frameWidth, frameHeight));
                            
                            if (_videoWriter.IsOpened())
                            {
                                System.Diagnostics.Debug.WriteLine($"Успешно открыт VideoWriter с кодеком: {codec.Name}");
                                videoWriterOpened = true;
                                _recordingFilePath = finalVideoPath; // Обновляем путь к файлу
                                break;
                            }
                        }
                        catch (Exception codecEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка при попытке использовать кодек {codec.Name}: {codecEx.Message}");
                            continue;
                        }
                    }
                    
                    if (!videoWriterOpened)
                    {
                        throw new Exception("Не удалось открыть VideoWriter ни с одним из доступных кодеков.");
                    }
                    
                    // Запускаем запись
                    _isRecording = true;
                    
                    // Инициализируем счетчики записи
                    _framesRecorded = 0;
                    _targetFps = fps;
                    _recordingStartTime = DateTime.Now;
                    
                    // Сбрасываем таймер записи
                    _recordingTime = TimeSpan.Zero;
                    txtRecordingTime.Text = "00:00";
                    
                    // Запускаем высокоточный таймер для записи кадров
                    int frameIntervalMs = (int)(1000.0 / fps);
                    _recordingFrameTimer = new System.Threading.Timer(RecordingFrameTimer_Tick, null, 0, frameIntervalMs);
                    
                    // Запускаем запись аудио
                    StartAudioRecording();
                    
                    // Запускаем таймер для обновления времени записи
                    _recordingTimer.Start();
                    
                    // Показываем индикатор записи
                    recordingIndicator.Visibility = Visibility.Visible;
                    StartRecordingIndicatorAnimation();
                    
                    // Показываем кнопку остановки и таймер
                    btnStopRecording.Visibility = Visibility.Visible;
                    txtRecordingTime.Visibility = Visibility.Visible;
                    
                    ShowStatus("Запись видео", "Идет запись...");
                }
                catch (Exception ex)
                {
                    _isRecording = false;
                    _videoWriter?.Dispose();
                    _videoWriter = null;
                    
                    throw new Exception($"Ошибка при инициализации записи видео: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Не удалось начать запись: {ex.Message}");
                CleanupRecording();
            }
        }
        
        private void StartRecordingIndicatorAnimation()
        {
            // Анимация мигания красного индикатора записи
            var blinkAnimation = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            blinkAnimation.Tick += (s, e) =>
            {
                if (!_isRecording)
                {
                    ((DispatcherTimer)s).Stop();
                    recordingIndicator.Visibility = Visibility.Collapsed;
                    return;
                }
                
                recordingIndicator.Visibility = recordingIndicator.Visibility == Visibility.Visible 
                    ? Visibility.Collapsed 
                    : Visibility.Visible;
            };
            blinkAnimation.Start();
        }
        
        private void BtnStopRecording_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                StopRecording();
            }
        }
        
        private void StopRecording()
        {
            if (!_isRecording)
                return;
            
            try
            {
                // Останавливаем таймер записи
                _recordingTimer.Stop();
                
                // Останавливаем высокоточный таймер записи кадров
                _recordingFrameTimer?.Dispose();
                _recordingFrameTimer = null;
                
                // Останавливаем запись аудио
                StopAudioRecording();
                
                // Освобождаем последний кадр
                lock (_frameLock)
                {
                    _lastFrame?.Dispose();
                    _lastFrame = null;
                }
                
                // Сбрасываем флаг записи и счетчики
                _isRecording = false;
                _framesRecorded = 0;
                _targetFps = 0;
                

                
                // Скрываем счетчик времени и индикатор записи
                txtRecordingTime.Visibility = Visibility.Collapsed;
                recordingIndicator.Visibility = Visibility.Collapsed;
                
                // Останавливаем запись видео
                if (_videoWriter != null && _videoWriter.IsOpened())
                {
                    _videoWriter.Release();
                    _videoWriter.Dispose();
                    _videoWriter = null;
                }
                
                // Логируем статистику записи
                var recordingDuration = DateTime.Now - _recordingStartTime;
                var actualFps = _framesRecorded / recordingDuration.TotalSeconds;
                System.Diagnostics.Debug.WriteLine($"Запись завершена: {_framesRecorded} кадров за {recordingDuration.TotalSeconds:F2} сек, FPS: {actualFps:F2} (целевой: {_targetFps})");
                
                // Проверяем, существует ли видеофайл и имеет ли он размер
                bool videoExists = File.Exists(_recordingFilePath) && new FileInfo(_recordingFilePath).Length > 0;
                
                if (!videoExists)
                {
                    ShowError("Не удалось записать видео. Пожалуйста, проверьте настройки камеры и попробуйте снова.");
                    CleanupRecording();
                    btnStartRecording.Visibility = Visibility.Visible;
                    return;
                }
                
                // Показываем статус
                ShowStatus("Подготовка видео", "Объединение видео и аудио...");
                
                // Запускаем обработку видео (слияние с аудио и приведение к MP4) в отдельном потоке
                Task.Run(() =>
                {
                    try
                    {
                        bool audioExists = !string.IsNullOrEmpty(_audioFilePath) && File.Exists(_audioFilePath) && new FileInfo(_audioFilePath).Length > 0;
                        string sourceVideoPath = _recordingFilePath;
                        string ext = Path.GetExtension(sourceVideoPath)?.ToLowerInvariant();

                        if (audioExists)
                        {
                            string dir = Path.GetDirectoryName(sourceVideoPath);
                            string nameNoExt = Path.GetFileNameWithoutExtension(sourceVideoPath);
                            string mergedPath = Path.Combine(dir ?? string.Empty, $"{nameNoExt}_with_audio.mp4");

                            // Сливаем видео и аудио с использованием FFmpeg
                            MergeVideoAndAudio(sourceVideoPath, _audioFilePath, mergedPath);

                            if (File.Exists(mergedPath) && new FileInfo(mergedPath).Length > 0)
                            {
                                try { File.Delete(sourceVideoPath); } catch { }
                                try { if (!string.IsNullOrEmpty(_audioFilePath)) File.Delete(_audioFilePath); } catch { }
                                _recordingFilePath = mergedPath; // Используем файл со звуком далее (воспроизведение/поделиться)
                            }
                        }

                        // Сохраняем видео локально и загружаем на Google Drive
                        Dispatcher.Invoke(() =>
                        {
                            SaveVideoLocally();
                            UploadVideoToGoogleDriveAsync();
                            FinalizeVideoProcessing();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ShowError($"Ошибка при обработке видео: {ex.Message}");
                            CleanupRecording();
                            btnStartRecording.Visibility = Visibility.Visible;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при остановке записи: {ex.Message}");
                CleanupRecording();
                btnStartRecording.Visibility = Visibility.Visible;
            }
        }
        
        // Метод для поиска видеофайла по всем возможным местам
        private void FindAndProcessVideoFile()
        {
            Dispatcher.Invoke(() => 
            {
                ShowStatus("Поиск временного файла", "Проверка созданных файлов...");
            });
            
            // Проверяем возможные места для временного файла
            string recordingsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recordings");
            string tempDir = System.IO.Path.Combine(recordingsDir, "temp");
            
            // Ищем последний созданный временный файл
            System.IO.FileInfo newestFile = null;
            
            // Проверяем в директории temp
            if (System.IO.Directory.Exists(tempDir))
            {
                var tempFiles = new System.IO.DirectoryInfo(tempDir).GetFiles("temp_*.avi");
                if (tempFiles.Length > 0)
                {
                    newestFile = tempFiles.OrderByDescending(f => f.LastWriteTime).First();
                    _tempVideoPath = newestFile.FullName;
                }
            }
            
            // Если не нашли в temp, ищем в корневой директории записей
            if (newestFile == null && System.IO.Directory.Exists(recordingsDir))
            {
                var rootTempFiles = new System.IO.DirectoryInfo(recordingsDir).GetFiles("temp_*.avi");
                if (rootTempFiles.Length > 0)
                {
                    newestFile = rootTempFiles.OrderByDescending(f => f.LastWriteTime).First();
                    _tempVideoPath = newestFile.FullName;
                }
            }
            
            // Если все еще не нашли, ищем любой временный avi файл
            if (newestFile == null)
            {
                // Ищем все .avi файлы в обеих директориях
                var allAviFiles = new List<System.IO.FileInfo>();
                
                if (System.IO.Directory.Exists(tempDir))
                    allAviFiles.AddRange(new System.IO.DirectoryInfo(tempDir).GetFiles("*.avi"));
                
                if (System.IO.Directory.Exists(recordingsDir))
                    allAviFiles.AddRange(new System.IO.DirectoryInfo(recordingsDir).GetFiles("*.avi"));
                
                if (allAviFiles.Count > 0)
                {
                    newestFile = allAviFiles.OrderByDescending(f => f.LastWriteTime).First();
                    _tempVideoPath = newestFile.FullName;
                }
            }
            
            // Если нашли временный файл
            if (!string.IsNullOrEmpty(_tempVideoPath) && System.IO.File.Exists(_tempVideoPath))
            {
                ProcessVideoFile(_tempVideoPath);
            }
            else
            {
                // Если временный файл не найден
                Dispatcher.Invoke(() =>
                {
                    ShowError("Временный видеофайл не найден. Запись не удалась.");
                    CleanupRecording();
                    btnStartRecording.Visibility = System.Windows.Visibility.Visible;
                });
            }
        }
        
        // Метод для обработки найденного видеофайла
        private void ProcessVideoFile(string videoFilePath)
        {
            Dispatcher.Invoke(() =>
            {
                ShowStatus("Файл найден", "Начинаем обработку видео...");
            });
            
            bool conversionSuccess = false;
            
            // Используем прямое копирование файла без обработки, если нет необходимости в объединении аудио
            if (!System.IO.File.Exists(_audioFilePath))
            {
                Dispatcher.Invoke(() =>
                {
                    ShowStatus("Сохранение видео", "Копирование файла...");
                });
                
                try
                {
                    // Просто копируем видео без конвертации, если аудио нет
                    string finalPath = _recordingFilePath.Replace(".mp4", ".avi");
                    System.IO.File.Copy(videoFilePath, finalPath, true);
                    _recordingFilePath = finalPath;
                    conversionSuccess = System.IO.File.Exists(_recordingFilePath);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowError($"Ошибка при копировании файла: {ex.Message}");
                    });
                }
            }
            else
            {
                // Проверяем, существует ли аудиофайл
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowStatus("Объединение аудио и видео", "Обработка может занять время...");
                    });
                    
                    // Объединение видео и звука с таймаутом
                    MergeVideoAndAudio(videoFilePath, _audioFilePath, _recordingFilePath);
                    conversionSuccess = System.IO.File.Exists(_recordingFilePath);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowError($"Ошибка при объединении: {ex.Message}. Копируем видео без звука.");
                    });
                    
                    // Если объединение не удалось, пробуем просто скопировать видео
                    try
                    {
                        string finalPath = _recordingFilePath.Replace(".mp4", ".avi");
                        System.IO.File.Copy(videoFilePath, finalPath, true);
                        _recordingFilePath = finalPath;
                        conversionSuccess = System.IO.File.Exists(_recordingFilePath);
                    }
                    catch (Exception copyEx)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ShowError($"Не удалось скопировать файл: {copyEx.Message}");
                        });
                    }
                }
                
                // Пытаемся удалить аудиофайл, но игнорируем ошибки
                try { System.IO.File.Delete(_audioFilePath); } catch { }
            }
            
            // Пытаемся удалить временный файл, но игнорируем ошибки
            try { System.IO.File.Delete(videoFilePath); } catch { }
            
            // Проверяем, успешно ли сохранено видео
            if (!conversionSuccess)
            {
                Dispatcher.Invoke(() =>
                {
                    ShowError("Не удалось сохранить видео. Попробуйте еще раз.");
                    CleanupRecording();
                    btnStartRecording.Visibility = System.Windows.Visibility.Visible;
                });
                return;
            }
            
            // Финальная обработка UI на главном потоке
            Dispatcher.Invoke(() =>
            {
                FinalizeVideoProcessing();
            });
        }
        
        private void SaveVideoLocally()
        {
            try
            {
                // Создаем папку для сохранения, если её нет
                string videosDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "VideoBooth");
                if (!Directory.Exists(videosDir))
                {
                    Directory.CreateDirectory(videosDir);
                }
                
                // Генерируем уникальное имя файла
                string fileName = $"VideoBooth_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
                string localVideoPath = Path.Combine(videosDir, fileName);
                
                // Копируем видео в локальную папку
                if (File.Exists(_recordingFilePath))
                {
                    File.Copy(_recordingFilePath, localVideoPath, true);
                    System.Diagnostics.Debug.WriteLine($"Видео сохранено локально: {localVideoPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении видео локально: {ex.Message}");
            }
        }
        
        private async void UploadVideoToGoogleDriveAsync()
        {
            // Запускаем загрузку в отдельном потоке, чтобы не блокировать UI
            await Task.Run(async () =>
            {
                try
                {
                    if (_driveService != null && _driveService.IsOnline && File.Exists(_recordingFilePath))
                    {
                        // Загружаем на Google Drive
                        string folderName = $"VideoBooth_{DateTime.Now:yyyyMMdd_HHmmss}";
                        var result = await _driveService.UploadVideoAsync(_recordingFilePath, folderName, _eventFolderId);
                        
                        // Сохраняем QR-код локально рядом с видео
                        if (result.QrCode != null)
                        {
                            string videosDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "VideoBooth");
                            string qrPath = Path.Combine(videosDir, $"VideoBooth_{DateTime.Now:yyyyMMdd_HHmmss}_qr.png");
                            result.QrCode.Save(qrPath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        
                        System.Diagnostics.Debug.WriteLine("Видео успешно загружено на Google Drive");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке видео на Google Drive: {ex.Message}");
                    // Не показываем ошибку пользователю, так как это фоновый процесс
                }
            });
        }
        
        // Метод для финализации обработки видео и обновления UI
        private void FinalizeVideoProcessing()
        {
            // Останавливаем анимацию индикатора записи
            recordingIndicator.Visibility = System.Windows.Visibility.Collapsed;
            
            // Проверяем, существует ли файл с видео
            if (System.IO.File.Exists(_recordingFilePath))
            {
                // Обновляем интерфейс
                btnStartRecording.Visibility = System.Windows.Visibility.Collapsed;
                btnStopRecording.Visibility = System.Windows.Visibility.Collapsed;
                btnReset.Visibility = System.Windows.Visibility.Visible;
                
                // Проверяем, можно ли показывать кнопку поделиться
                UpdateShareButtonVisibility();
                
                btnPlayPause.Visibility = System.Windows.Visibility.Visible;
                btnTogglePreview.Visibility = System.Windows.Visibility.Visible;
                
                // Показываем превью по умолчанию, но готовы к переключению на видео
                _showingPreview = true;
                imgPreview.Visibility = System.Windows.Visibility.Visible;
                mediaPlayer.Visibility = System.Windows.Visibility.Collapsed;
                
                try
                {
                    // Убеждаемся, что файл доступен и имеет корректный формат
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(_recordingFilePath);
                    if (fileInfo.Length == 0)
                    {
                        ShowError("Созданный файл видео пуст. Повторите запись.");
                        return;
                    }
                    
                    ShowStatus("Подготовка видео", "Проверка совместимости формата...");
                    
                    // Определяем, нужно ли конвертировать видео для совместимости
                    // Проверяем формат видео по расширению
                    string ext = System.IO.Path.GetExtension(_recordingFilePath).ToLower();
                    
                    if (ext == ".avi")
                    {
                        // AVI формат может вызывать проблемы с кодеками, конвертируем сразу
                        ConvertToCompatibleFormat(_recordingFilePath);
                        return;
                    }
                    
                    // Пробуем воспроизвести оригинальное видео
                    ShowStatus("Загрузка видео", "Подготовка медиаплеера...");
                    
                    // Создаем безопасный путь к видео
                    string safeVideoPath = _recordingFilePath;
                    
                    // Проверяем, может ли MediaPlayer открыть файл
                    System.Uri videoUri = new System.Uri(safeVideoPath, System.UriKind.Absolute);
                    
                    // Сбрасываем существующие ресурсы
                    mediaPlayer.Close();
                    mediaPlayer.Source = null;
                    GC.Collect(); // Принудительно освобождаем ресурсы, которые могли остаться от предыдущего воспроизведения
                    
                    // Настраиваем MediaPlayer
                    mediaPlayer.LoadedBehavior = System.Windows.Controls.MediaState.Manual;
                    mediaPlayer.UnloadedBehavior = System.Windows.Controls.MediaState.Stop;
                    mediaPlayer.Volume = 1.0;
                    mediaPlayer.ScrubbingEnabled = true;
                    mediaPlayer.IsMuted = false;
                    
                    // Показываем медиаэлемент перед установкой источника
                    mediaPlayer.Visibility = System.Windows.Visibility.Visible;
                    
                    // Создаем небольшую задержку перед загрузкой видео
                    System.Threading.Tasks.Task.Delay(800).ContinueWith(_ => 
                    {
                        Dispatcher.Invoke(() => 
                        {
                            try
                            {
                                // Устанавливаем источник и сразу запускаем воспроизведение
                                mediaPlayer.Source = videoUri;
                                System.Threading.Thread.Sleep(200); // Небольшая задержка для загрузки
                                mediaPlayer.Play();
                                _isPlaying = true;
                                btnPlayPause.Content = "⏸";
                                ShowStatus("Видео готово", "Вы можете поделиться видео или записать новое");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Ошибка при воспроизведении: {ex.Message}");
                                // При ошибке пытаемся конвертировать видео
                                ConvertToCompatibleFormat(_recordingFilePath);
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при воспроизведении: {ex.Message}");
                    // При ошибке пытаемся конвертировать видео
                    ConvertToCompatibleFormat(_recordingFilePath);
                }
            }
            else
            {
                ShowError("Не удалось сохранить видео. Попробуйте еще раз.");
                CleanupRecording();
                btnStartRecording.Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        // Метод для конвертации видео в более совместимый формат
        private void ConvertToCompatibleFormat(string videoPath)
        {
            if (string.IsNullOrEmpty(videoPath) || !System.IO.File.Exists(videoPath))
            {
                ShowError("Видеофайл не найден.");
                return;
            }
            
            try
            {
                // Создаем временный файл для конвертированного видео
                string outputPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    $"UnifiedPhotoBooth_compatible_{System.IO.Path.GetFileNameWithoutExtension(videoPath)}.mp4");
                
                // Путь к FFmpeg
                string ffmpegPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
                
                if (!System.IO.File.Exists(ffmpegPath))
                {
                    ShowError("FFmpeg не найден. Невозможно конвертировать видео.");
                    return;
                }
                
                // Параметры для конвертации в совместимый формат
                // Используем параметры кодирования, которые точно поддерживаются MediaElement
                string arguments = $"-i \"{videoPath}\" -c:v libx264 -preset ultrafast " +
                                  $"-profile:v baseline -level 3.0 -pix_fmt yuv420p " +
                                  $"-c:a aac -strict experimental -b:a 128k " +
                                  $"-f mp4 \"{outputPath}\"";
                
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                };
                
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
                {
                    string error = process.StandardError.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($"FFmpeg output: {error}");
                    
                    if (process.WaitForExit(60000)) // Ждем максимум 1 минуту
                    {
                        if (process.ExitCode == 0 && System.IO.File.Exists(outputPath))
                        {
                            System.Diagnostics.Debug.WriteLine($"Видео успешно конвертировано: {outputPath}");
                            
                            // Воспроизводим конвертированное видео
                            Dispatcher.Invoke(() =>
                            {
                                ShowStatus("Загрузка видео", "Воспроизведение конвертированного видео...");
                                
                                try
                                {
                                    // Очищаем текущий источник и закрываем
                                    mediaPlayer.Close();
                                    mediaPlayer.Source = null;
                                    
                                    // Загружаем новый файл
                                    mediaPlayer.Source = new System.Uri(outputPath);
                                    mediaPlayer.Play();
                                    _isPlaying = true;
                                    btnPlayPause.Content = "⏸";
                                    mediaPlayer.Visibility = System.Windows.Visibility.Visible;
                                    
                                    ShowStatus("Видео готово", "Воспроизведение...");
                                }
                                catch (Exception ex)
                                {
                                    ShowError($"Ошибка при воспроизведении конвертированного видео: {ex.Message}");
                                }
                            });
                        }
                        else
                        {
                            ShowError($"Не удалось конвертировать видео. Код выхода: {process.ExitCode}");
                        }
                    }
                    else
                    {
                        ShowError("Превышено время ожидания при конвертации видео.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при конвертации видео: {ex.Message}");
            }
        }
        
        private void MergeVideoAndAudio(string videoPath, string audioPath, string outputPath)
        {
            try
            {
                Dispatcher.Invoke(() => {
                    ShowStatus("Запуск FFmpeg", "Подготовка к объединению видео и аудио...");
                });
                
                // Путь к FFmpeg
                string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
                
                // Проверяем, существует ли FFmpeg
                if (!File.Exists(ffmpegPath))
                {
                    throw new FileNotFoundException("FFmpeg не найден. Пожалуйста, установите FFmpeg.");
                }
                
                // Проверяем существование директории для выходного файла
                string outputDir = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                
                // Полные пути к файлам
                videoPath = Path.GetFullPath(videoPath);
                audioPath = Path.GetFullPath(audioPath);
                outputPath = Path.GetFullPath(outputPath);
                
                // Проверяем, существуют ли исходные файлы
                if (!File.Exists(videoPath))
                {
                    throw new FileNotFoundException($"Видео файл не найден: {videoPath}");
                }
                
                if (!File.Exists(audioPath))
                {
                    throw new FileNotFoundException($"Аудио файл не найден: {audioPath}");
                }
                
                Dispatcher.Invoke(() => {
                    ShowStatus("Обработка видео", "Объединение видео и аудио. Это может занять несколько минут...");
                });
                
                // Выбираем стратегию: если исходник MP4 — пробуем копировать видео-поток; иначе перекодируем видео
                bool isMp4Source = string.Equals(Path.GetExtension(videoPath), ".mp4", StringComparison.OrdinalIgnoreCase);
                string argumentsPrimary = isMp4Source
                    ? $"-y -i \"{videoPath}\" -i \"{audioPath}\" -map 0:v:0 -map 1:a:0 -c:v copy -c:a aac -b:a 192k -shortest \"{outputPath}\""
                    : $"-y -i \"{videoPath}\" -i \"{audioPath}\" -map 0:v:0 -map 1:a:0 -c:v libx264 -preset veryfast -pix_fmt yuv420p -c:a aac -b:a 192k -shortest \"{outputPath}\"";
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = argumentsPrimary,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    string err = process.StandardError.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine($"FFmpeg mux log: {err}");
                    if (!process.WaitForExit(180000))
                    {
                        try { process.Kill(); } catch { }
                        throw new TimeoutException("FFmpeg превысил время ожидания при объединении аудио и видео");
                    }
                    if (process.ExitCode != 0 || !File.Exists(outputPath) || new FileInfo(outputPath).Length == 0)
                    {
                        // Пытаемся форсировать перекодирование как резервный вариант
                        var fallbackInfo = new ProcessStartInfo
                        {
                            FileName = ffmpegPath,
                            Arguments = $"-y -i \"{videoPath}\" -i \"{audioPath}\" -map 0:v:0 -map 1:a:0 -c:v libx264 -preset veryfast -pix_fmt yuv420p -c:a aac -b:a 192k -shortest \"{outputPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true
                        };
                        using (var p2 = Process.Start(fallbackInfo))
                        {
                            string err2 = p2.StandardError.ReadToEnd();
                            System.Diagnostics.Debug.WriteLine($"FFmpeg fallback mux log: {err2}");
                            if (!p2.WaitForExit(180000) || p2.ExitCode != 0)
                            {
                                throw new Exception("Не удалось объединить видео и аудио с помощью FFmpeg");
                            }
                        }
                    }
                }
                
                if (!File.Exists(outputPath))
                {
                    throw new FileNotFoundException("Выходной файл не был создан FFmpeg");
                }
                
                Dispatcher.Invoke(() => {
                    ShowStatus("Завершение", "Видео со звуком готово!");
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при объединении видео и аудио: {ex.Message}", ex);
            }
        }
        
        private void ConvertVideoToMp4(string inputPath, string outputPath)
        {
            try
            {
                Dispatcher.Invoke(() => {
                    ShowStatus("Конвертация видео", "Преобразование формата видео...");
                });
                
                // Путь к FFmpeg
                string ffmpegPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
                
                // Проверяем, существует ли FFmpeg
                if (!System.IO.File.Exists(ffmpegPath))
                {
                    throw new System.IO.FileNotFoundException("FFmpeg не найден. Пожалуйста, установите FFmpeg.");
                }
                
                // Проверяем существование директории для выходного файла
                string outputDir = System.IO.Path.GetDirectoryName(outputPath);
                if (!System.IO.Directory.Exists(outputDir))
                {
                    System.IO.Directory.CreateDirectory(outputDir);
                }
                
                // Полные пути к файлам
                inputPath = System.IO.Path.GetFullPath(inputPath);
                outputPath = System.IO.Path.GetFullPath(outputPath);
                
                // Проверяем, существуют ли исходные файлы
                if (!System.IO.File.Exists(inputPath))
                {
                    throw new System.IO.FileNotFoundException($"Видео файл не найден: {inputPath}");
                }
                
                // Создаем команду для FFmpeg с более простыми параметрами
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-i \"{inputPath}\" -c:v libx264 -preset ultrafast -crf 28 \"{outputPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                
                // Запускаем процесс
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
                {
                    // Ждем завершения процесса с таймаутом 3 минуты
                    if (!process.WaitForExit(180000))
                    {
                        process.Kill();
                        throw new System.TimeoutException("Процесс FFmpeg превысил время ожидания");
                    }
                    
                    // Проверяем код выхода
                    if (process.ExitCode != 0)
                    {
                        // Пробуем просто скопировать файл
                        string copyPath = outputPath.Replace(".mp4", ".avi");
                        System.IO.File.Copy(inputPath, copyPath, true);
                        
                        // Проверяем, что файл скопирован
                        if (System.IO.File.Exists(copyPath))
                        {
                            return; // Используем этот файл
                        }
                        
                        // Читаем ошибку
                        string error = process.StandardError.ReadToEnd();
                        throw new System.Exception($"FFmpeg завершился с ошибкой (код: {process.ExitCode}): {error}");
                    }
                }
                
                // Проверяем, создался ли файл
                if (!System.IO.File.Exists(outputPath))
                {
                    throw new System.IO.FileNotFoundException("Выходной файл не был создан FFmpeg");
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Ошибка при конвертации видео: {ex.Message}", ex);
            }
        }
        
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Останавливаем проигрывание
                if (mediaPlayer.Source != null)
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer.Source = null;
                }
                
                mediaPlayer.Visibility = Visibility.Collapsed;
                
                // Скрываем QR-код, если он был показан
                imgQrCode.Visibility = Visibility.Collapsed;
                txtQrCodeUrl.Visibility = Visibility.Collapsed;
                
                // Показываем превью
                imgPreview.Visibility = Visibility.Visible;
                
                // Обновляем интерфейс
                btnStartRecording.Visibility = Visibility.Visible;
                btnReset.Visibility = Visibility.Collapsed;
                btnShare.Visibility = Visibility.Collapsed;
                btnPlayPause.Visibility = Visibility.Collapsed;
                btnTogglePreview.Visibility = Visibility.Collapsed;
                
                // Удаляем временные файлы
                try
                {
                    if (!string.IsNullOrEmpty(_recordingFilePath) && System.IO.File.Exists(_recordingFilePath))
                        System.IO.File.Delete(_recordingFilePath);
                    
                    _recordingFilePath = null;
                }
                catch
                {
                    // Игнорируем ошибки при удалении файлов
                }
                
                ShowStatus("Готов к записи", "Нажмите 'Начать запись', чтобы записать видео");
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка при сбросе: {ex.Message}");
            }
        }
        
        private async void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_recordingFilePath) || !System.IO.File.Exists(_recordingFilePath))
            {
                ShowError("Файл видео не найден.");
                return;
            }
            
            try
            {
                ShowStatus("Загрузка видео", "Пожалуйста, подождите...");
                
                // Отключаем кнопки на время загрузки
                btnReset.IsEnabled = false;
                btnShare.IsEnabled = false;
                btnPlayPause.IsEnabled = false;
                btnTogglePreview.IsEnabled = false;
                
                // Загружаем видео в Google Drive
                string folderName = $"VideoBooth_{System.DateTime.Now:yyyyMMdd_HHmmss}";
                var result = await _driveService.UploadVideoAsync(_recordingFilePath, folderName, _eventFolderId, false, _lastUniversalFolderId);
                
                // Сохраняем ID папки для возможного повторного использования
                _lastUniversalFolderId = result.FolderId;
                
                // Конвертируем QR-код в формат для отображения
                BitmapImage qrImage = new BitmapImage();
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    result.QrCode.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    qrImage.BeginInit();
                    qrImage.CacheOption = BitmapCacheOption.OnLoad;
                    qrImage.StreamSource = ms;
                    qrImage.EndInit();
                }
                
                // Отображаем только QR-код без ссылки
                imgQrCode.Source = qrImage;
                imgQrCode.Visibility = Visibility.Visible;
                
                // Скрываем текстовое поле ссылки
                txtQrCodeUrl.Visibility = Visibility.Collapsed;
                
                ShowStatus("Видео загружено", "Отсканируйте QR-код для доступа к видео");
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка при загрузке видео: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Ошибка в BtnShare_Click: {ex.Message}");
            }
            finally
            {
                // Включаем кнопки
                btnReset.IsEnabled = true;
                btnShare.IsEnabled = true;
                btnPlayPause.IsEnabled = true;
                btnTogglePreview.IsEnabled = true;
            }
        }
        
        private void BtnTogglePreview_Click(object sender, RoutedEventArgs e)
        {
            if (_showingPreview)
            {
                // Переключаемся на видео
                _showingPreview = false;
                imgPreview.Visibility = Visibility.Collapsed;
                mediaPlayer.Visibility = Visibility.Visible;
                btnTogglePreview.Content = "📷";
                btnTogglePreview.ToolTip = "Показать превью камеры";
                
                // Воспроизводим видео, если оно не воспроизводится
                if (mediaPlayer.Source != null && !_isPlaying)
                {
                    mediaPlayer.Play();
                    _isPlaying = true;
                    btnPlayPause.Content = "⏸";
                }
            }
            else
            {
                // Переключаемся на превью
                _showingPreview = true;
                imgPreview.Visibility = Visibility.Visible;
                mediaPlayer.Visibility = Visibility.Collapsed;
                btnTogglePreview.Content = "📹";
                btnTogglePreview.ToolTip = "Показать записанное видео";
                
                // Останавливаем воспроизведение видео
                if (_isPlaying)
                {
                    mediaPlayer.Pause();
                    _isPlaying = false;
                    btnPlayPause.Content = "▶";
                }
            }
        }
        
        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, есть ли что воспроизводить
                if (mediaPlayer.Source == null)
                {
                    ShowStatus("Ошибка", "Нет видео для воспроизведения");
                    return;
                }
                
                // Проверяем существование файла
                string videoPath = mediaPlayer.Source.LocalPath;
                if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
                {
                    ShowStatus("Ошибка", "Видеофайл не найден");
                    mediaPlayer.Source = null;
                    return;
                }
                
                // Проверяем размер файла
                var fileInfo = new FileInfo(videoPath);
                if (fileInfo.Length == 0)
                {
                    ShowStatus("Ошибка", "Видеофайл пуст");
                    mediaPlayer.Source = null;
                    return;
                }
                
                if (_isPlaying)
                {
                    mediaPlayer.Pause();
                    _isPlaying = false;
                    btnPlayPause.Content = "▶";
                }
                else
                {
                    mediaPlayer.Play();
                    _isPlaying = true;
                    btnPlayPause.Content = "⏸";
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при воспроизведении: {ex.Message}");
                _isPlaying = false;
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
            }
        }
        
        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Возвращаем в начало вместо перезапуска воспроизведения
                mediaPlayer.Position = System.TimeSpan.Zero;
                _isPlaying = false;
                btnPlayPause.Content = "▶";
                
                ShowStatus("Видео завершено", "Нажмите ▶ для повторного просмотра");
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка при воспроизведении: {ex.Message}");
            }
        }
        
        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowStatus("Видео загружено", "Воспроизведение начато");
                _isPlaying = true;
                btnPlayPause.Content = "⏸";
                
                // Проверяем, что видео видно
                if (mediaPlayer.Visibility != Visibility.Visible)
                {
                    mediaPlayer.Visibility = Visibility.Visible;
                }
                
                // Запускаем воспроизведение
                mediaPlayer.Play();
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка при загрузке видео: {ex.Message}");
            }
        }
        
        private void MediaPlayer_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            string errorMessage = e.ErrorException?.Message ?? "Неизвестная ошибка";
            
            // Логируем ошибку, но не открываем сразу внешний проигрыватель
            System.Diagnostics.Debug.WriteLine($"Ошибка воспроизведения видео: {errorMessage}");
            
            // Проверяем тип ошибки
            if (errorMessage.Contains("0xC00D109B") || errorMessage.Contains("кодек") || errorMessage.Contains("codec"))
            {
                // Если проблема с кодеком, пробуем конвертировать в более совместимый формат
                ShowStatus("Проблема с форматом видео", "Конвертация в совместимый формат...");
                
                try
                {
                    // Создаем новый формат видео для проигрывания
                    ConvertToCompatibleFormat(_recordingFilePath);
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка при конвертации видео: {ex.Message}");
                }
            }
            else
            {
                ShowError($"Ошибка воспроизведения: {errorMessage}");
            }
        }
        
        private void ShowStatus(string status, string info)
        {
            txtStatus.Text = status;
            txtInfo.Text = info;
        }
        
        private void ShowError(string message)
        {
            ShowStatus("Ошибка", message);
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            _countdownValue--;
            
            if (_countdownValue > 0)
            {
                txtCountdown.Text = _countdownValue.ToString();
            }
            else
            {
                _countdownTimer.Stop();
                txtCountdown.Visibility = Visibility.Collapsed;
                
                // Начинаем запись после обратного отсчета
                _onCountdownComplete?.Invoke();
            }
        }
        
        // Добавляем новый метод для корректировки соотношения сторон кадра
        private Mat AdjustAspectRatio(Mat frame, double targetAspectRatio)
        {
            double frameAspectRatio = (double)frame.Width / frame.Height;
            
            // Проверяем режим поворота - если выбран "Без поворота", то не применяем обрезку для вертикального режима
            string rotationMode = SettingsWindow.AppSettings.RotationMode;
            bool isDefaultRotation = rotationMode == "Без поворота";
            
            // Если соотношение сторон близко к целевому или это режим "Без поворота", возвращаем исходный кадр
            if (System.Math.Abs(frameAspectRatio - targetAspectRatio) < 0.01 || isDefaultRotation)
            {
                return frame;
            }
            
            // Создаем ROI для обрезки изображения до нужного соотношения сторон
            Mat croppedFrame;
            
            if (frameAspectRatio > targetAspectRatio) // Кадр шире, чем нужно
            {
                int newWidth = (int)(frame.Height * targetAspectRatio);
                int xOffset = (frame.Width - newWidth) / 2;
                var roi = new Rect(xOffset, 0, newWidth, frame.Height);
                croppedFrame = new Mat(frame, roi);
            }
            else // Кадр уже, чем нужно
            {
                int newHeight = (int)(frame.Width / targetAspectRatio);
                int yOffset = (frame.Height - newHeight) / 2;
                var roi = new Rect(0, yOffset, frame.Width, newHeight);
                croppedFrame = new Mat(frame, roi);
            }
            
            return croppedFrame;
        }
        
        // Добавляю метод для проверки возможности делиться
        private void UpdateShareButtonVisibility()
        {
            // Показываем кнопку "Поделиться" только если есть интернет
            btnShare.Visibility = _driveService.IsOnline ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void CleanupRecording()
        {
            _isRecording = false;
            _framesRecorded = 0;
            _targetFps = 0;
            
            // Останавливаем высокоточный таймер записи кадров
            _recordingFrameTimer?.Dispose();
            _recordingFrameTimer = null;
            
            // Останавливаем запись аудио
            StopAudioRecording();
            
            // Освобождаем последний кадр
            lock (_frameLock)
            {
                _lastFrame?.Dispose();
                _lastFrame = null;
            }

            _recordingTimer.Stop();
            txtRecordingTime.Visibility = Visibility.Collapsed;
            
            _videoWriter?.Release();
            _videoWriter?.Dispose();
            _videoWriter = null;
            
            recordingIndicator.Visibility = Visibility.Collapsed;
            btnStopRecording.Visibility = Visibility.Collapsed;
            btnStartRecording.Visibility = Visibility.Visible;
        }
        
        private void StartAudioRecording()
        {
            try
            {
                if (SettingsWindow.AppSettings.UseMicrophone)
                {
                    _audioCapture = new WaveInEvent
                    {
                        DeviceNumber = SettingsWindow.AppSettings.MicrophoneIndex,
                        WaveFormat = new WaveFormat(44100, 16, 1),
                        BufferMilliseconds = 50
                    };
                    
                    _audioCapture.DataAvailable += AudioCapture_DataAvailable;
                    _audioWriter = new WaveFileWriter(_audioFilePath, _audioCapture.WaveFormat);
                    _audioCapture.StartRecording();
                    
                    System.Diagnostics.Debug.WriteLine("Запись аудио начата");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при запуске записи аудио: {ex.Message}");
            }
        }
        
        private void StopAudioRecording()
        {
            try
            {
                if (_audioCapture != null)
                {
                    _audioCapture.StopRecording();
                    _audioCapture.DataAvailable -= AudioCapture_DataAvailable;
                    _audioCapture.Dispose();
                    _audioCapture = null;
                }
                
                if (_audioWriter != null)
                {
                    _audioWriter.Dispose();
                    _audioWriter = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при остановке записи аудио: {ex.Message}");
            }
        }
        
        private void AudioCapture_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                if (_audioWriter != null && e.BytesRecorded > 0)
                {
                    _audioWriter.Write(e.Buffer, 0, e.BytesRecorded);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при записи аудио данных: {ex.Message}");
            }
        }
    }
} 