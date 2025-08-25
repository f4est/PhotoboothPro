using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000014 RID: 20
	public partial class App : Application
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600008E RID: 142 RVA: 0x00005434 File Offset: 0x00003634
		// (remove) Token: 0x0600008F RID: 143 RVA: 0x00005468 File Offset: 0x00003668
		public static event EventHandler ApplicationExit;

		// Token: 0x06000090 RID: 144 RVA: 0x0000549C File Offset: 0x0000369C
		private bool IsRunningAsAdministrator()
		{
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000054C4 File Offset: 0x000036C4
		private void RestartAsAdministrator()
		{
			string exeName = Process.GetCurrentProcess().MainModule.FileName;
			ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
			{
				UseShellExecute = true,
				Verb = "runas"
			};
			try
			{
				Process.Start(startInfo);
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005518 File Offset: 0x00003718
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			if (!this.IsRunningAsAdministrator())
			{
				this.RestartAsAdministrator();
				Application.Current.Shutdown();
			}
			AppDomain.CurrentDomain.UnhandledException += this.GlobalExceptionHandler;
			base.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000556C File Offset: 0x0000376C
		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			MessageBox.Show("Task Exception: " + e.Exception.Message);
			e.SetObserved();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005590 File Offset: 0x00003790
		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			string errorDetails = this.GetExceptionDetails(e.Exception);
			if (e.Exception is FileNotFoundException)
			{
				MessageBoxWindow.CreateWindow("File Not Found", "Please Control File and Restart App as Administrator", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			if (e.Exception is OutOfMemoryException)
			{
				MessageBoxWindow.CreateWindow("Out Of Memory", "Please try closing other applications to free up memory or increase your system's available resources.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			this.HandleError(errorDetails, "");
			e.Handled = true;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005618 File Offset: 0x00003818
		private void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
			string errorDetails = this.GetExceptionDetails(ex);
			if (ex is UnauthorizedAccessException || ex is IOException)
			{
				MessageBoxWindow.CreateWindow("Need To Run s Administrator", "Please Restart App as Administrator", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			this.HandleError(errorDetails, "Critical");
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005674 File Offset: 0x00003874
		private bool CanSendMail()
		{
			DateTime lastDatetime = this._lastDatetime;
			if ((DateTime.Now - this._lastDatetime).TotalMinutes > 5.0)
			{
				this._lastDatetime = DateTime.Now;
				return true;
			}
			return false;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000056BC File Offset: 0x000038BC
		private void HandleError(string message, string errorMode = "")
		{
			App.<HandleError>d__11 <HandleError>d__;
			<HandleError>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleError>d__.<>4__this = this;
			<HandleError>d__.message = message;
			<HandleError>d__.errorMode = errorMode;
			<HandleError>d__.<>1__state = -1;
			<HandleError>d__.<>t__builder.Start<App.<HandleError>d__11>(ref <HandleError>d__);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005704 File Offset: 0x00003904
		public static void ExitApp()
		{
			App.<ExitApp>d__12 <ExitApp>d__;
			<ExitApp>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ExitApp>d__.<>1__state = -1;
			<ExitApp>d__.<>t__builder.Start<App.<ExitApp>d__12>(ref <ExitApp>d__);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005734 File Offset: 0x00003934
		private string GetExceptionDetails(Exception ex)
		{
			string userInfo = "";
			if (SessionData.accountInfo != null)
			{
				userInfo = SessionData.accountInfo.email + "\n" + SessionData.accountInfo.accessToken + "\n";
			}
			if (EventManagementPage.GetCurrentEvent() != null)
			{
				OperationalEvent operationalEvent = EventManagementPage.GetCurrentEvent();
				userInfo = string.Concat(new string[]
				{
					userInfo,
					"Event ID: ",
					operationalEvent.IndexID,
					"\n",
					operationalEvent.EventName
				});
			}
			if (!string.IsNullOrEmpty(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
			{
				userInfo = userInfo + "\nBuild Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			string errorMessage = "Hata Mesajı: " + ex.Message + "\n";
			string stackTrace = "Stack Trace:\n" + ex.StackTrace + "\n";
			if (ex.InnerException != null)
			{
				errorMessage = errorMessage + "İç Hata Mesajı: " + ex.InnerException.Message + "\n";
				errorMessage = errorMessage + "İç Hata Stack Trace:\n" + ex.InnerException.StackTrace + "\n";
			}
			return userInfo + errorMessage + stackTrace;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005860 File Offset: 0x00003A60
		protected override void OnExit(ExitEventArgs e)
		{
			if (ExtensionMethod.IsInternetAvailable())
			{
				try
				{
					new Thread(delegate()
					{
						App.<>c.<<OnExit>b__14_0>d <<OnExit>b__14_0>d;
						<<OnExit>b__14_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
						<<OnExit>b__14_0>d.<>1__state = -1;
						<<OnExit>b__14_0>d.<>t__builder.Start<App.<>c.<<OnExit>b__14_0>d>(ref <<OnExit>b__14_0>d);
					}).Start();
				}
				catch (Exception ex)
				{
				}
			}
			MailClass.isForceQuit = (TwilioHelper.isForceQuit = (FTPUpload.isForceQuit = true));
			GoProCameraControlClass.StopKeepAliveThread();
			WebcamControlClass.StopWebcam();
			MessageBoxWindow.KillCloseMessageBoxWithTimeThread();
			base.OnExit(e);
			EventHandler applicationExit = App.ApplicationExit;
			if (applicationExit != null)
			{
				applicationExit(this, EventArgs.Empty);
			}
			Application.Current.Shutdown();
			Environment.Exit(0);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005900 File Offset: 0x00003B00
		private void Application_Startup(object sender, StartupEventArgs e)
		{
		}

		// Token: 0x04000078 RID: 120
		private DateTime _lastDatetime;
	}
}
