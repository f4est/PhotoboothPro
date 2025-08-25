using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000CD RID: 205
	public class StripeCheckoutHelper
	{
		// Token: 0x06000AFD RID: 2813 RVA: 0x000402A7 File Offset: 0x0003E4A7
		public StripeCheckoutHelper(string platformSecretKey)
		{
			this._client = new StripeClient(platformSecretKey, null, null, null, null, null, null);
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x000402C4 File Offset: 0x0003E4C4
		public Session CreateCheckoutSessionAdHoc(string connectedAccountId, long amountMinor, string currency, string productName, string successUrl, string cancelUrl, bool saveCardForFutureOffSession)
		{
			SessionService sessionSvc = new SessionService(this._client);
			SessionLineItemPriceDataOptions priceData = new SessionLineItemPriceDataOptions();
			priceData.Currency = currency;
			priceData.UnitAmount = new long?(amountMinor);
			priceData.ProductData = new SessionLineItemPriceDataProductDataOptions
			{
				Name = productName
			};
			SessionLineItemOptions line = new SessionLineItemOptions();
			line.PriceData = priceData;
			line.Quantity = new long?(1L);
			SessionCreateOptions createOpts = new SessionCreateOptions();
			createOpts.Mode = "payment";
			createOpts.SuccessUrl = successUrl;
			createOpts.CancelUrl = cancelUrl;
			createOpts.LineItems = new List<SessionLineItemOptions>
			{
				line
			};
			Dictionary<string, string> md = new Dictionary<string, string>();
			md["origin"] = "adhoc";
			createOpts.Metadata = md;
			if (saveCardForFutureOffSession)
			{
				createOpts.PaymentIntentData = new SessionPaymentIntentDataOptions
				{
					SetupFutureUsage = "off_session"
				};
			}
			return sessionSvc.Create(createOpts, new RequestOptions
			{
				StripeAccount = connectedAccountId,
				IdempotencyKey = Guid.NewGuid().ToString()
			});
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x000403C8 File Offset: 0x0003E5C8
		public Task<PaymentWaitResult> WaitPaidAsync(string connectedAccountId, string sessionId, TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
		{
			StripeCheckoutHelper.<WaitPaidAsync>d__3 <WaitPaidAsync>d__;
			<WaitPaidAsync>d__.<>t__builder = AsyncTaskMethodBuilder<PaymentWaitResult>.Create();
			<WaitPaidAsync>d__.<>4__this = this;
			<WaitPaidAsync>d__.connectedAccountId = connectedAccountId;
			<WaitPaidAsync>d__.sessionId = sessionId;
			<WaitPaidAsync>d__.timeout = timeout;
			<WaitPaidAsync>d__.cancellationToken = cancellationToken;
			<WaitPaidAsync>d__.<>1__state = -1;
			<WaitPaidAsync>d__.<>t__builder.Start<StripeCheckoutHelper.<WaitPaidAsync>d__3>(ref <WaitPaidAsync>d__);
			return <WaitPaidAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x0004042C File Offset: 0x0003E62C
		public Task<Session> ExpireSessionAsync(string connectedAccountId, string sessionId)
		{
			SessionService svc = new SessionService(this._client);
			RequestOptions ro = new RequestOptions
			{
				StripeAccount = connectedAccountId
			};
			return svc.ExpireAsync(sessionId, null, ro, default(CancellationToken));
		}

		// Token: 0x04000A56 RID: 2646
		private readonly StripeClient _client;
	}
}
