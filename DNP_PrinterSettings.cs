using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Класс для управления специфическими настройками принтера DNP DS-RX1
    /// </summary>
    public class DNP_PrinterSettings
    {
        // Константы для типа медиа в принтере DNP DS-RX1
        public enum PrintMediaType
        {
            Glossy = 0,  // Глянцевая поверхность
            Matte = 1    // Матовая поверхность
        }

        // Константы для типа печати в принтере DNP DS-RX1
        public enum PrintType
        {
            Standard = 0,    // Стандартная печать
            FineDeep = 1,    // Печать с улучшенным глубоким черным
            FineLight = 2    // Печать с улучшенной светлой частью
        }

        // Константы для размера печати
        public enum PrintSize
        {
            Postcard = 0,    // 10x15 см (Postcard)
            LSize = 1,       // 9x13 см (L size)
            Custom = 2       // Пользовательский размер
        }

        // Константы для качества печати
        public enum PrintQuality
        {
            HighSpeed = 0,   // Высокая скорость
            HighQuality = 1  // Высокое качество
        }

        // Класс для хранения настроек печати DNP
        public class DNPSettings
        {
            public PrintMediaType MediaType { get; set; } = PrintMediaType.Glossy;
            public PrintType Type { get; set; } = PrintType.Standard;
            public PrintSize Size { get; set; } = PrintSize.Postcard;
            public PrintQuality Quality { get; set; } = PrintQuality.HighQuality;
            public int Copies { get; set; } = 1;
            public int Sharpness { get; set; } = 0;       // От -10 до 10
            public int Brightness { get; set; } = 0;      // От -10 до 10
            public int Contrast { get; set; } = 0;        // От -10 до 10
            public bool EnableColorMatching { get; set; } = true;
            public bool MirrorPrint { get; set; } = false;
            public bool MultiCut { get; set; } = false;   // Режим мультирезки (для печати нескольких маленьких фото)
            public int CustomWidth { get; set; } = 1446;  // В единицах 0.1 мм (1446 = 14.46 см)
            public int CustomHeight { get; set; } = 2151; // В единицах 0.1 мм (2151 = 21.51 см)
        }

        // Константы для доступа к дополнительным свойствам печати DNP DS-RX1
        private const uint DM_PAPERSIZE = 0x00000002;
        private const uint DM_PAPERWIDTH = 0x00000008;
        private const uint DM_PAPERLENGTH = 0x00000010;
        private const short DMPAPER_USER = 256;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public uint dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;

            // Конструктор для создания DEVMODE
            public DEVMODE()
            {
                dmDeviceName = new string(' ', 32);
                dmFormName = new string(' ', 32);
                dmSize = (short)Marshal.SizeOf(this);
            }
        }

        // Получение списка доступных принтеров
        public static List<string> GetAvailablePrinters()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToList();
        }

        // Проверка, является ли принтер DNP DS-RX1
        public static bool IsDNPPrinter(string printerName)
        {
            return printerName.Contains("DNP") && (printerName.Contains("DS-RX1") || printerName.Contains("RX1"));
        }

        // Применение настроек DNP к документу для печати
        public static void ApplyDNPSettings(PrintDocument printDoc, DNPSettings settings)
        {
            try
            {
                // Установка основных настроек печати
                printDoc.PrinterSettings.Copies = (short)settings.Copies;

                // Получаем ссылку на объект DEVMODE для дополнительных настроек
                IntPtr devModePointer = printDoc.PrinterSettings.GetHdevmode();
                DEVMODE devMode = (DEVMODE)Marshal.PtrToStructure(devModePointer, typeof(DEVMODE));

                // Установка типа медиа (глянцевая/матовая)
                devMode.dmMediaType = (uint)settings.MediaType;
                
                // Установка пользовательского размера, если выбран Custom
                if (settings.Size == PrintSize.Custom)
                {
                    devMode.dmFields |= DM_PAPERSIZE | DM_PAPERWIDTH | DM_PAPERLENGTH;
                    devMode.dmPaperSize = DMPAPER_USER;
                    devMode.dmPaperWidth = (short)settings.CustomWidth;
                    devMode.dmPaperLength = (short)settings.CustomHeight;
                }
                else if (settings.Size == PrintSize.Postcard)
                {
                    devMode.dmFields |= DM_PAPERSIZE;
                    devMode.dmPaperSize = 5; // Postcard (10x15 см)
                }
                else if (settings.Size == PrintSize.LSize)
                {
                    devMode.dmFields |= DM_PAPERSIZE;
                    devMode.dmPaperSize = 3; // L size (9x13 см)
                }

                // Копируем измененные настройки обратно в printDoc
                Marshal.StructureToPtr(devMode, devModePointer, true);
                printDoc.PrinterSettings.SetHdevmode(devModePointer);
                
                // Освобождаем ресурсы
                Marshal.FreeHGlobal(devModePointer);

                // Для установки специфичных настроек DNP DS-RX1 используем диалоговое окно печати
                // Обратите внимание, что некоторые настройки могут быть доступны только через диалог печати
                // или через специальный драйвер DNP, поставляемый с принтером
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при применении настроек принтера DNP: {ex.Message}", ex);
            }
        }

        // Получение текущих настроек принтера DNP
        public static DNPSettings GetCurrentDNPSettings(string printerName)
        {
            DNPSettings settings = new DNPSettings();

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;

                if (printDoc.PrinterSettings.IsValid)
                {
                    // Получаем текущие настройки принтера
                    IntPtr devModePointer = printDoc.PrinterSettings.GetHdevmode();
                    DEVMODE devMode = (DEVMODE)Marshal.PtrToStructure(devModePointer, typeof(DEVMODE));

                    // Получаем тип медиа
                    settings.MediaType = (PrintMediaType)devMode.dmMediaType;

                    // Получаем размер бумаги
                    if (devMode.dmPaperSize == DMPAPER_USER)
                    {
                        settings.Size = PrintSize.Custom;
                        settings.CustomWidth = devMode.dmPaperWidth;
                        settings.CustomHeight = devMode.dmPaperLength;
                    }
                    else if (devMode.dmPaperSize == 5) // Postcard
                    {
                        settings.Size = PrintSize.Postcard;
                    }
                    else if (devMode.dmPaperSize == 3) // L Size
                    {
                        settings.Size = PrintSize.LSize;
                    }

                    // Освобождаем ресурсы
                    Marshal.FreeHGlobal(devModePointer);
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем настройки по умолчанию
                System.Windows.MessageBox.Show($"Ошибка при получении настроек принтера DNP: {ex.Message}", 
                                             "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return settings;
        }

        // Создание тестовой страницы для принтера DNP DS-RX1
        public static System.Drawing.Bitmap CreateDNPTestPage()
        {
            // Создаем тестовую страницу для принтера DNP DS-RX1
            // Размер изображения по умолчанию 1446x2151 пикселей (10x15 см при 300 dpi)
            System.Drawing.Bitmap testImage = new System.Drawing.Bitmap(1446, 2151);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(testImage))
            {
                // Заливаем фон
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, testImage.Width, testImage.Height);

                // Настраиваем высокое качество отрисовки
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // Рамка вокруг изображения
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black, 10), 10, 10, testImage.Width - 20, testImage.Height - 20);

                // Заголовок
                using (System.Drawing.Font titleFont = new System.Drawing.Font("Arial", 48, System.Drawing.FontStyle.Bold))
                {
                    var centerFormat = new System.Drawing.StringFormat();
                    centerFormat.Alignment = System.Drawing.StringAlignment.Center;
                    g.DrawString("DNP DS-RX1 TEST", titleFont, System.Drawing.Brushes.Black, 
                                new System.Drawing.Rectangle(0, 100, testImage.Width, 100), 
                                centerFormat);
                }

                // Добавляем цветные квадраты (основные цвета)
                int squareSize = 200;
                int startY = 300;
                int margin = 20;
                
                // Первый ряд
                g.FillRectangle(System.Drawing.Brushes.Red, margin, startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Green, margin + squareSize + margin, startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Blue, margin + 2 * (squareSize + margin), startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Cyan, margin + 3 * (squareSize + margin), startY, squareSize, squareSize);

                // Второй ряд
                g.FillRectangle(System.Drawing.Brushes.Magenta, margin, startY + squareSize + margin, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Yellow, margin + squareSize + margin, startY + squareSize + margin, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Black, margin + 2 * (squareSize + margin), startY + squareSize + margin, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.White, margin + 3 * (squareSize + margin), startY + squareSize + margin, squareSize, squareSize);
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black, 2), 
                               margin + 3 * (squareSize + margin), startY + squareSize + margin, squareSize, squareSize);

                // Добавляем градиент для проверки плавных переходов
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new System.Drawing.Point(0, 0),
                    new System.Drawing.Point(testImage.Width, 0),
                    System.Drawing.Color.Black,
                    System.Drawing.Color.White))
                {
                    g.FillRectangle(brush, margin, startY + 2 * (squareSize + margin), testImage.Width - 2 * margin, 100);
                }

                // Добавляем текст с информацией
                using (System.Drawing.Font infoFont = new System.Drawing.Font("Arial", 20))
                {
                    g.DrawString($"Printer: DNP DS-RX1", infoFont, System.Drawing.Brushes.Black, 
                                margin, startY + 2 * (squareSize + margin) + 150);
                    g.DrawString($"Date: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}", infoFont, System.Drawing.Brushes.Black, 
                                margin, startY + 2 * (squareSize + margin) + 190);
                    g.DrawString($"Resolution: 300 DPI", infoFont, System.Drawing.Brushes.Black, 
                                margin, startY + 2 * (squareSize + margin) + 230);
                    g.DrawString($"Size: 10x15 cm (Postcard)", infoFont, System.Drawing.Brushes.Black, 
                                margin, startY + 2 * (squareSize + margin) + 270);
                }

                // Добавляем текстовый паттерн для проверки качества печати мелкого текста
                using (System.Drawing.Font smallFont = new System.Drawing.Font("Courier New", 8))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        string line = "1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz";
                        g.DrawString(line, smallFont, System.Drawing.Brushes.Black, 
                                    margin, startY + 2 * (squareSize + margin) + 320 + i * 15);
                    }
                }
            }

            return testImage;
        }
    }
}