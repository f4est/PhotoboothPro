using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using Stripe.Checkout;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000CC RID: 204
	public static class StripeController
	{
		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x000400B8 File Offset: 0x0003E2B8
		public static bool PaymentPrintActive
		{
			get
			{
				return Settings.GetValueBoolean("paymentactive").Value && (StripeController.paymentOption == PaymentOption.Printdownload || StripeController.paymentOption == PaymentOption.Printonly);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x000400EC File Offset: 0x0003E2EC
		public static PaymentOption paymentOption
		{
			get
			{
				string paymentPageString = Settings.GetValueString("paymenttime");
				return (PaymentOption)Enum.Parse(typeof(PaymentOption), paymentPageString.Replace(" ", "").Replace("&", ""));
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000AF8 RID: 2808 RVA: 0x00040138 File Offset: 0x0003E338
		public static PaymentScreen paymentScreen
		{
			get
			{
				string paymentPageString = Settings.GetValueString("paymentscreen");
				return (PaymentScreen)Enum.Parse(typeof(PaymentScreen), paymentPageString.Replace(" ", "").Replace("&", ""));
			}
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00040184 File Offset: 0x0003E384
		public static bool IsPayment(PaymentScreen thisPage)
		{
			if (MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth)
			{
				return false;
			}
			string paymentPageString = Settings.GetValueString("paymentscreen");
			if (!Settings.GetValueBoolean("paymentactive").Value)
			{
				return false;
			}
			if (string.IsNullOrEmpty(paymentPageString))
			{
				return false;
			}
			PaymentScreen paymentPage = (PaymentScreen)Enum.Parse(typeof(PaymentScreen), paymentPageString.Replace(" ", "").Replace("&", ""));
			return thisPage == paymentPage;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x00040204 File Offset: 0x0003E404
		public static Session CreatePaymentSession(long amount, string productName, long multipler = 1L)
		{
			amount *= multipler;
			string platformSecret = Settings.GetValueString("stripesecret");
			string connectedAcctId = Settings.GetValueString("stripeaccountid");
			StripeCheckoutHelper helper = new StripeCheckoutHelper(platformSecret);
			string currency = "usd";
			return helper.CreateCheckoutSessionAdHoc(connectedAcctId, amount, currency, productName, "https://activationshare.com/Payment/Success", "https://activationshare.com/Payment/Failure", false);
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00040254 File Offset: 0x0003E454
		public static Task<PaymentWaitResult> Payment(Session session, CancellationToken cancellationToken)
		{
			StripeController.<Payment>d__9 <Payment>d__;
			<Payment>d__.<>t__builder = AsyncTaskMethodBuilder<PaymentWaitResult>.Create();
			<Payment>d__.session = session;
			<Payment>d__.cancellationToken = cancellationToken;
			<Payment>d__.<>1__state = -1;
			<Payment>d__.<>t__builder.Start<StripeController.<Payment>d__9>(ref <Payment>d__);
			return <Payment>d__.<>t__builder.Task;
		}

		// Token: 0x04000A55 RID: 2645
		public static int paymentLimitTime = 2;
	}
}
