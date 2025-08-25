using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008A RID: 138
	public class DiscordMessageModals
	{
		// Token: 0x0200022C RID: 556
		public class Attachment
		{
			// Token: 0x170001C4 RID: 452
			// (get) Token: 0x06000ECE RID: 3790 RVA: 0x0005BC3D File Offset: 0x00059E3D
			// (set) Token: 0x06000ECF RID: 3791 RVA: 0x0005BC45 File Offset: 0x00059E45
			public string id { get; set; }

			// Token: 0x170001C5 RID: 453
			// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x0005BC4E File Offset: 0x00059E4E
			// (set) Token: 0x06000ED1 RID: 3793 RVA: 0x0005BC56 File Offset: 0x00059E56
			public string filename { get; set; }

			// Token: 0x170001C6 RID: 454
			// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x0005BC5F File Offset: 0x00059E5F
			// (set) Token: 0x06000ED3 RID: 3795 RVA: 0x0005BC67 File Offset: 0x00059E67
			public int size { get; set; }

			// Token: 0x170001C7 RID: 455
			// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x0005BC70 File Offset: 0x00059E70
			// (set) Token: 0x06000ED5 RID: 3797 RVA: 0x0005BC78 File Offset: 0x00059E78
			public string url { get; set; }

			// Token: 0x170001C8 RID: 456
			// (get) Token: 0x06000ED6 RID: 3798 RVA: 0x0005BC81 File Offset: 0x00059E81
			// (set) Token: 0x06000ED7 RID: 3799 RVA: 0x0005BC89 File Offset: 0x00059E89
			public string proxy_url { get; set; }

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06000ED8 RID: 3800 RVA: 0x0005BC92 File Offset: 0x00059E92
			// (set) Token: 0x06000ED9 RID: 3801 RVA: 0x0005BC9A File Offset: 0x00059E9A
			public int width { get; set; }

			// Token: 0x170001CA RID: 458
			// (get) Token: 0x06000EDA RID: 3802 RVA: 0x0005BCA3 File Offset: 0x00059EA3
			// (set) Token: 0x06000EDB RID: 3803 RVA: 0x0005BCAB File Offset: 0x00059EAB
			public int height { get; set; }

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x06000EDC RID: 3804 RVA: 0x0005BCB4 File Offset: 0x00059EB4
			// (set) Token: 0x06000EDD RID: 3805 RVA: 0x0005BCBC File Offset: 0x00059EBC
			public string content_type { get; set; }

			// Token: 0x170001CC RID: 460
			// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0005BCC5 File Offset: 0x00059EC5
			// (set) Token: 0x06000EDF RID: 3807 RVA: 0x0005BCCD File Offset: 0x00059ECD
			public string placeholder { get; set; }

			// Token: 0x170001CD RID: 461
			// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x0005BCD6 File Offset: 0x00059ED6
			// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x0005BCDE File Offset: 0x00059EDE
			public int placeholder_version { get; set; }
		}

		// Token: 0x0200022D RID: 557
		public class Author
		{
			// Token: 0x170001CE RID: 462
			// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0005BCEF File Offset: 0x00059EEF
			// (set) Token: 0x06000EE4 RID: 3812 RVA: 0x0005BCF7 File Offset: 0x00059EF7
			public string id { get; set; }

			// Token: 0x170001CF RID: 463
			// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x0005BD00 File Offset: 0x00059F00
			// (set) Token: 0x06000EE6 RID: 3814 RVA: 0x0005BD08 File Offset: 0x00059F08
			public string username { get; set; }

			// Token: 0x170001D0 RID: 464
			// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0005BD11 File Offset: 0x00059F11
			// (set) Token: 0x06000EE8 RID: 3816 RVA: 0x0005BD19 File Offset: 0x00059F19
			public string avatar { get; set; }

			// Token: 0x170001D1 RID: 465
			// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x0005BD22 File Offset: 0x00059F22
			// (set) Token: 0x06000EEA RID: 3818 RVA: 0x0005BD2A File Offset: 0x00059F2A
			public string discriminator { get; set; }

			// Token: 0x170001D2 RID: 466
			// (get) Token: 0x06000EEB RID: 3819 RVA: 0x0005BD33 File Offset: 0x00059F33
			// (set) Token: 0x06000EEC RID: 3820 RVA: 0x0005BD3B File Offset: 0x00059F3B
			public int public_flags { get; set; }

			// Token: 0x170001D3 RID: 467
			// (get) Token: 0x06000EED RID: 3821 RVA: 0x0005BD44 File Offset: 0x00059F44
			// (set) Token: 0x06000EEE RID: 3822 RVA: 0x0005BD4C File Offset: 0x00059F4C
			public int premium_type { get; set; }

			// Token: 0x170001D4 RID: 468
			// (get) Token: 0x06000EEF RID: 3823 RVA: 0x0005BD55 File Offset: 0x00059F55
			// (set) Token: 0x06000EF0 RID: 3824 RVA: 0x0005BD5D File Offset: 0x00059F5D
			public int flags { get; set; }

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x0005BD66 File Offset: 0x00059F66
			// (set) Token: 0x06000EF2 RID: 3826 RVA: 0x0005BD6E File Offset: 0x00059F6E
			public bool bot { get; set; }

			// Token: 0x170001D6 RID: 470
			// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0005BD77 File Offset: 0x00059F77
			// (set) Token: 0x06000EF4 RID: 3828 RVA: 0x0005BD7F File Offset: 0x00059F7F
			public object banner { get; set; }

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0005BD88 File Offset: 0x00059F88
			// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0005BD90 File Offset: 0x00059F90
			public object accent_color { get; set; }

			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0005BD99 File Offset: 0x00059F99
			// (set) Token: 0x06000EF8 RID: 3832 RVA: 0x0005BDA1 File Offset: 0x00059FA1
			public object global_name { get; set; }

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0005BDAA File Offset: 0x00059FAA
			// (set) Token: 0x06000EFA RID: 3834 RVA: 0x0005BDB2 File Offset: 0x00059FB2
			public object avatar_decoration_data { get; set; }

			// Token: 0x170001DA RID: 474
			// (get) Token: 0x06000EFB RID: 3835 RVA: 0x0005BDBB File Offset: 0x00059FBB
			// (set) Token: 0x06000EFC RID: 3836 RVA: 0x0005BDC3 File Offset: 0x00059FC3
			public object banner_color { get; set; }
		}

		// Token: 0x0200022E RID: 558
		public class Channels
		{
		}

		// Token: 0x0200022F RID: 559
		public class Component
		{
			// Token: 0x170001DB RID: 475
			// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0005BDDC File Offset: 0x00059FDC
			// (set) Token: 0x06000F00 RID: 3840 RVA: 0x0005BDE4 File Offset: 0x00059FE4
			public int type { get; set; }

			// Token: 0x170001DC RID: 476
			// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0005BDED File Offset: 0x00059FED
			// (set) Token: 0x06000F02 RID: 3842 RVA: 0x0005BDF5 File Offset: 0x00059FF5
			public List<DiscordMessageModals.Component> components { get; set; }

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0005BDFE File Offset: 0x00059FFE
			// (set) Token: 0x06000F04 RID: 3844 RVA: 0x0005BE06 File Offset: 0x0005A006
			public string custom_id { get; set; }

			// Token: 0x170001DE RID: 478
			// (get) Token: 0x06000F05 RID: 3845 RVA: 0x0005BE0F File Offset: 0x0005A00F
			// (set) Token: 0x06000F06 RID: 3846 RVA: 0x0005BE17 File Offset: 0x0005A017
			public int style { get; set; }

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x06000F07 RID: 3847 RVA: 0x0005BE20 File Offset: 0x0005A020
			// (set) Token: 0x06000F08 RID: 3848 RVA: 0x0005BE28 File Offset: 0x0005A028
			public string label { get; set; }

			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x06000F09 RID: 3849 RVA: 0x0005BE31 File Offset: 0x0005A031
			// (set) Token: 0x06000F0A RID: 3850 RVA: 0x0005BE39 File Offset: 0x0005A039
			public DiscordMessageModals.Emoji emoji { get; set; }

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x06000F0B RID: 3851 RVA: 0x0005BE42 File Offset: 0x0005A042
			// (set) Token: 0x06000F0C RID: 3852 RVA: 0x0005BE4A File Offset: 0x0005A04A
			public string url { get; set; }
		}

		// Token: 0x02000230 RID: 560
		public class Emoji
		{
			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0005BE5B File Offset: 0x0005A05B
			// (set) Token: 0x06000F0F RID: 3855 RVA: 0x0005BE63 File Offset: 0x0005A063
			public string name { get; set; }

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0005BE6C File Offset: 0x0005A06C
			// (set) Token: 0x06000F11 RID: 3857 RVA: 0x0005BE74 File Offset: 0x0005A074
			public string id { get; set; }
		}

		// Token: 0x02000231 RID: 561
		public class Members
		{
		}

		// Token: 0x02000232 RID: 562
		public class Mention
		{
			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0005BE8D File Offset: 0x0005A08D
			// (set) Token: 0x06000F15 RID: 3861 RVA: 0x0005BE95 File Offset: 0x0005A095
			public string id { get; set; }

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0005BE9E File Offset: 0x0005A09E
			// (set) Token: 0x06000F17 RID: 3863 RVA: 0x0005BEA6 File Offset: 0x0005A0A6
			public string username { get; set; }

			// Token: 0x170001E6 RID: 486
			// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0005BEAF File Offset: 0x0005A0AF
			// (set) Token: 0x06000F19 RID: 3865 RVA: 0x0005BEB7 File Offset: 0x0005A0B7
			public object avatar { get; set; }

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0005BEC0 File Offset: 0x0005A0C0
			// (set) Token: 0x06000F1B RID: 3867 RVA: 0x0005BEC8 File Offset: 0x0005A0C8
			public string discriminator { get; set; }

			// Token: 0x170001E8 RID: 488
			// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0005BED1 File Offset: 0x0005A0D1
			// (set) Token: 0x06000F1D RID: 3869 RVA: 0x0005BED9 File Offset: 0x0005A0D9
			public int public_flags { get; set; }

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0005BEE2 File Offset: 0x0005A0E2
			// (set) Token: 0x06000F1F RID: 3871 RVA: 0x0005BEEA File Offset: 0x0005A0EA
			public int premium_type { get; set; }

			// Token: 0x170001EA RID: 490
			// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0005BEF3 File Offset: 0x0005A0F3
			// (set) Token: 0x06000F21 RID: 3873 RVA: 0x0005BEFB File Offset: 0x0005A0FB
			public int flags { get; set; }

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0005BF04 File Offset: 0x0005A104
			// (set) Token: 0x06000F23 RID: 3875 RVA: 0x0005BF0C File Offset: 0x0005A10C
			public object banner { get; set; }

			// Token: 0x170001EC RID: 492
			// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0005BF15 File Offset: 0x0005A115
			// (set) Token: 0x06000F25 RID: 3877 RVA: 0x0005BF1D File Offset: 0x0005A11D
			public object accent_color { get; set; }

			// Token: 0x170001ED RID: 493
			// (get) Token: 0x06000F26 RID: 3878 RVA: 0x0005BF26 File Offset: 0x0005A126
			// (set) Token: 0x06000F27 RID: 3879 RVA: 0x0005BF2E File Offset: 0x0005A12E
			public string global_name { get; set; }

			// Token: 0x170001EE RID: 494
			// (get) Token: 0x06000F28 RID: 3880 RVA: 0x0005BF37 File Offset: 0x0005A137
			// (set) Token: 0x06000F29 RID: 3881 RVA: 0x0005BF3F File Offset: 0x0005A13F
			public object avatar_decoration_data { get; set; }

			// Token: 0x170001EF RID: 495
			// (get) Token: 0x06000F2A RID: 3882 RVA: 0x0005BF48 File Offset: 0x0005A148
			// (set) Token: 0x06000F2B RID: 3883 RVA: 0x0005BF50 File Offset: 0x0005A150
			public object banner_color { get; set; }
		}

		// Token: 0x02000233 RID: 563
		public class MessageReference
		{
			// Token: 0x170001F0 RID: 496
			// (get) Token: 0x06000F2D RID: 3885 RVA: 0x0005BF61 File Offset: 0x0005A161
			// (set) Token: 0x06000F2E RID: 3886 RVA: 0x0005BF69 File Offset: 0x0005A169
			public string channel_id { get; set; }

			// Token: 0x170001F1 RID: 497
			// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0005BF72 File Offset: 0x0005A172
			// (set) Token: 0x06000F30 RID: 3888 RVA: 0x0005BF7A File Offset: 0x0005A17A
			public string message_id { get; set; }

			// Token: 0x170001F2 RID: 498
			// (get) Token: 0x06000F31 RID: 3889 RVA: 0x0005BF83 File Offset: 0x0005A183
			// (set) Token: 0x06000F32 RID: 3890 RVA: 0x0005BF8B File Offset: 0x0005A18B
			public string guild_id { get; set; }
		}

		// Token: 0x02000234 RID: 564
		public class ReferencedMessage
		{
			// Token: 0x170001F3 RID: 499
			// (get) Token: 0x06000F34 RID: 3892 RVA: 0x0005BF9C File Offset: 0x0005A19C
			// (set) Token: 0x06000F35 RID: 3893 RVA: 0x0005BFA4 File Offset: 0x0005A1A4
			public string id { get; set; }

			// Token: 0x170001F4 RID: 500
			// (get) Token: 0x06000F36 RID: 3894 RVA: 0x0005BFAD File Offset: 0x0005A1AD
			// (set) Token: 0x06000F37 RID: 3895 RVA: 0x0005BFB5 File Offset: 0x0005A1B5
			public int type { get; set; }

			// Token: 0x170001F5 RID: 501
			// (get) Token: 0x06000F38 RID: 3896 RVA: 0x0005BFBE File Offset: 0x0005A1BE
			// (set) Token: 0x06000F39 RID: 3897 RVA: 0x0005BFC6 File Offset: 0x0005A1C6
			public string content { get; set; }

			// Token: 0x170001F6 RID: 502
			// (get) Token: 0x06000F3A RID: 3898 RVA: 0x0005BFCF File Offset: 0x0005A1CF
			// (set) Token: 0x06000F3B RID: 3899 RVA: 0x0005BFD7 File Offset: 0x0005A1D7
			public string channel_id { get; set; }

			// Token: 0x170001F7 RID: 503
			// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0005BFE0 File Offset: 0x0005A1E0
			// (set) Token: 0x06000F3D RID: 3901 RVA: 0x0005BFE8 File Offset: 0x0005A1E8
			public DiscordMessageModals.Author author { get; set; }

			// Token: 0x170001F8 RID: 504
			// (get) Token: 0x06000F3E RID: 3902 RVA: 0x0005BFF1 File Offset: 0x0005A1F1
			// (set) Token: 0x06000F3F RID: 3903 RVA: 0x0005BFF9 File Offset: 0x0005A1F9
			public List<DiscordMessageModals.Attachment> attachments { get; set; }

			// Token: 0x170001F9 RID: 505
			// (get) Token: 0x06000F40 RID: 3904 RVA: 0x0005C002 File Offset: 0x0005A202
			// (set) Token: 0x06000F41 RID: 3905 RVA: 0x0005C00A File Offset: 0x0005A20A
			public List<object> embeds { get; set; }

			// Token: 0x170001FA RID: 506
			// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0005C013 File Offset: 0x0005A213
			// (set) Token: 0x06000F43 RID: 3907 RVA: 0x0005C01B File Offset: 0x0005A21B
			public List<DiscordMessageModals.Mention> mentions { get; set; }

			// Token: 0x170001FB RID: 507
			// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0005C024 File Offset: 0x0005A224
			// (set) Token: 0x06000F45 RID: 3909 RVA: 0x0005C02C File Offset: 0x0005A22C
			public List<object> mention_roles { get; set; }

			// Token: 0x170001FC RID: 508
			// (get) Token: 0x06000F46 RID: 3910 RVA: 0x0005C035 File Offset: 0x0005A235
			// (set) Token: 0x06000F47 RID: 3911 RVA: 0x0005C03D File Offset: 0x0005A23D
			public bool pinned { get; set; }

			// Token: 0x170001FD RID: 509
			// (get) Token: 0x06000F48 RID: 3912 RVA: 0x0005C046 File Offset: 0x0005A246
			// (set) Token: 0x06000F49 RID: 3913 RVA: 0x0005C04E File Offset: 0x0005A24E
			public bool mention_everyone { get; set; }

			// Token: 0x170001FE RID: 510
			// (get) Token: 0x06000F4A RID: 3914 RVA: 0x0005C057 File Offset: 0x0005A257
			// (set) Token: 0x06000F4B RID: 3915 RVA: 0x0005C05F File Offset: 0x0005A25F
			public bool tts { get; set; }

			// Token: 0x170001FF RID: 511
			// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0005C068 File Offset: 0x0005A268
			// (set) Token: 0x06000F4D RID: 3917 RVA: 0x0005C070 File Offset: 0x0005A270
			public DateTime timestamp { get; set; }

			// Token: 0x17000200 RID: 512
			// (get) Token: 0x06000F4E RID: 3918 RVA: 0x0005C079 File Offset: 0x0005A279
			// (set) Token: 0x06000F4F RID: 3919 RVA: 0x0005C081 File Offset: 0x0005A281
			public object edited_timestamp { get; set; }

			// Token: 0x17000201 RID: 513
			// (get) Token: 0x06000F50 RID: 3920 RVA: 0x0005C08A File Offset: 0x0005A28A
			// (set) Token: 0x06000F51 RID: 3921 RVA: 0x0005C092 File Offset: 0x0005A292
			public int flags { get; set; }

			// Token: 0x17000202 RID: 514
			// (get) Token: 0x06000F52 RID: 3922 RVA: 0x0005C09B File Offset: 0x0005A29B
			// (set) Token: 0x06000F53 RID: 3923 RVA: 0x0005C0A3 File Offset: 0x0005A2A3
			public List<DiscordMessageModals.Component> components { get; set; }

			// Token: 0x17000203 RID: 515
			// (get) Token: 0x06000F54 RID: 3924 RVA: 0x0005C0AC File Offset: 0x0005A2AC
			// (set) Token: 0x06000F55 RID: 3925 RVA: 0x0005C0B4 File Offset: 0x0005A2B4
			public DiscordMessageModals.Resolved resolved { get; set; }
		}

		// Token: 0x02000235 RID: 565
		public class Resolved
		{
			// Token: 0x17000204 RID: 516
			// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0005C0C5 File Offset: 0x0005A2C5
			// (set) Token: 0x06000F58 RID: 3928 RVA: 0x0005C0CD File Offset: 0x0005A2CD
			public DiscordMessageModals.Users users { get; set; }

			// Token: 0x17000205 RID: 517
			// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0005C0D6 File Offset: 0x0005A2D6
			// (set) Token: 0x06000F5A RID: 3930 RVA: 0x0005C0DE File Offset: 0x0005A2DE
			public DiscordMessageModals.Members members { get; set; }

			// Token: 0x17000206 RID: 518
			// (get) Token: 0x06000F5B RID: 3931 RVA: 0x0005C0E7 File Offset: 0x0005A2E7
			// (set) Token: 0x06000F5C RID: 3932 RVA: 0x0005C0EF File Offset: 0x0005A2EF
			public DiscordMessageModals.Channels channels { get; set; }

			// Token: 0x17000207 RID: 519
			// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0005C0F8 File Offset: 0x0005A2F8
			// (set) Token: 0x06000F5E RID: 3934 RVA: 0x0005C100 File Offset: 0x0005A300
			public DiscordMessageModals.Roles roles { get; set; }
		}

		// Token: 0x02000236 RID: 566
		public class Roles
		{
		}

		// Token: 0x02000237 RID: 567
		public class DiscordMessage
		{
			// Token: 0x17000208 RID: 520
			// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0005C119 File Offset: 0x0005A319
			// (set) Token: 0x06000F62 RID: 3938 RVA: 0x0005C121 File Offset: 0x0005A321
			public string id { get; set; }

			// Token: 0x17000209 RID: 521
			// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0005C12A File Offset: 0x0005A32A
			// (set) Token: 0x06000F64 RID: 3940 RVA: 0x0005C132 File Offset: 0x0005A332
			public int type { get; set; }

			// Token: 0x1700020A RID: 522
			// (get) Token: 0x06000F65 RID: 3941 RVA: 0x0005C13B File Offset: 0x0005A33B
			// (set) Token: 0x06000F66 RID: 3942 RVA: 0x0005C143 File Offset: 0x0005A343
			public string content { get; set; }

			// Token: 0x1700020B RID: 523
			// (get) Token: 0x06000F67 RID: 3943 RVA: 0x0005C14C File Offset: 0x0005A34C
			// (set) Token: 0x06000F68 RID: 3944 RVA: 0x0005C154 File Offset: 0x0005A354
			public string channel_id { get; set; }

			// Token: 0x1700020C RID: 524
			// (get) Token: 0x06000F69 RID: 3945 RVA: 0x0005C15D File Offset: 0x0005A35D
			// (set) Token: 0x06000F6A RID: 3946 RVA: 0x0005C165 File Offset: 0x0005A365
			public DiscordMessageModals.Author author { get; set; }

			// Token: 0x1700020D RID: 525
			// (get) Token: 0x06000F6B RID: 3947 RVA: 0x0005C16E File Offset: 0x0005A36E
			// (set) Token: 0x06000F6C RID: 3948 RVA: 0x0005C176 File Offset: 0x0005A376
			public List<DiscordMessageModals.Attachment> attachments { get; set; }

			// Token: 0x1700020E RID: 526
			// (get) Token: 0x06000F6D RID: 3949 RVA: 0x0005C17F File Offset: 0x0005A37F
			// (set) Token: 0x06000F6E RID: 3950 RVA: 0x0005C187 File Offset: 0x0005A387
			public List<object> embeds { get; set; }

			// Token: 0x1700020F RID: 527
			// (get) Token: 0x06000F6F RID: 3951 RVA: 0x0005C190 File Offset: 0x0005A390
			// (set) Token: 0x06000F70 RID: 3952 RVA: 0x0005C198 File Offset: 0x0005A398
			public List<DiscordMessageModals.Mention> mentions { get; set; }

			// Token: 0x17000210 RID: 528
			// (get) Token: 0x06000F71 RID: 3953 RVA: 0x0005C1A1 File Offset: 0x0005A3A1
			// (set) Token: 0x06000F72 RID: 3954 RVA: 0x0005C1A9 File Offset: 0x0005A3A9
			public List<object> mention_roles { get; set; }

			// Token: 0x17000211 RID: 529
			// (get) Token: 0x06000F73 RID: 3955 RVA: 0x0005C1B2 File Offset: 0x0005A3B2
			// (set) Token: 0x06000F74 RID: 3956 RVA: 0x0005C1BA File Offset: 0x0005A3BA
			public bool pinned { get; set; }

			// Token: 0x17000212 RID: 530
			// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0005C1C3 File Offset: 0x0005A3C3
			// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0005C1CB File Offset: 0x0005A3CB
			public bool mention_everyone { get; set; }

			// Token: 0x17000213 RID: 531
			// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0005C1D4 File Offset: 0x0005A3D4
			// (set) Token: 0x06000F78 RID: 3960 RVA: 0x0005C1DC File Offset: 0x0005A3DC
			public bool tts { get; set; }

			// Token: 0x17000214 RID: 532
			// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0005C1E5 File Offset: 0x0005A3E5
			// (set) Token: 0x06000F7A RID: 3962 RVA: 0x0005C1ED File Offset: 0x0005A3ED
			public DateTime timestamp { get; set; }

			// Token: 0x17000215 RID: 533
			// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0005C1F6 File Offset: 0x0005A3F6
			// (set) Token: 0x06000F7C RID: 3964 RVA: 0x0005C1FE File Offset: 0x0005A3FE
			public object edited_timestamp { get; set; }

			// Token: 0x17000216 RID: 534
			// (get) Token: 0x06000F7D RID: 3965 RVA: 0x0005C207 File Offset: 0x0005A407
			// (set) Token: 0x06000F7E RID: 3966 RVA: 0x0005C20F File Offset: 0x0005A40F
			public int flags { get; set; }

			// Token: 0x17000217 RID: 535
			// (get) Token: 0x06000F7F RID: 3967 RVA: 0x0005C218 File Offset: 0x0005A418
			// (set) Token: 0x06000F80 RID: 3968 RVA: 0x0005C220 File Offset: 0x0005A420
			public List<DiscordMessageModals.Component> components { get; set; }

			// Token: 0x17000218 RID: 536
			// (get) Token: 0x06000F81 RID: 3969 RVA: 0x0005C229 File Offset: 0x0005A429
			// (set) Token: 0x06000F82 RID: 3970 RVA: 0x0005C231 File Offset: 0x0005A431
			public DiscordMessageModals.Resolved resolved { get; set; }

			// Token: 0x17000219 RID: 537
			// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0005C23A File Offset: 0x0005A43A
			// (set) Token: 0x06000F84 RID: 3972 RVA: 0x0005C242 File Offset: 0x0005A442
			public DiscordMessageModals.MessageReference message_reference { get; set; }

			// Token: 0x1700021A RID: 538
			// (get) Token: 0x06000F85 RID: 3973 RVA: 0x0005C24B File Offset: 0x0005A44B
			// (set) Token: 0x06000F86 RID: 3974 RVA: 0x0005C253 File Offset: 0x0005A453
			public DiscordMessageModals.ReferencedMessage referenced_message { get; set; }
		}

		// Token: 0x02000238 RID: 568
		public class Users
		{
		}
	}
}
