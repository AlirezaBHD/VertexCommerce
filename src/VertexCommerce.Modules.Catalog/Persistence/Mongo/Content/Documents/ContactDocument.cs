using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class ContactDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000002");

    [BsonElement("header")]
    public ContactHeaderSection Header { get; set; } = new();

    [BsonElement("phones")]
    public List<ContactPhoneItem> Phones { get; set; } = new();

    [BsonElement("emails")]
    public List<ContactEmailItem> Emails { get; set; } = new();

    [BsonElement("workingHours")]
    public ContactWorkingHoursSection WorkingHours { get; set; } = new();

    [BsonElement("location")]
    public ContactLocationSection Location { get; set; } = new();

    [BsonElement("photos")]
    public List<ContactStorePhotoItem> Photos { get; set; } = new();

    [BsonElement("socials")]
    public List<ContactSocialItem> Socials { get; set; } = new();

    [BsonElement("faqs")]
    public List<ContactFaqItem> Faqs { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    public static ContactDocument CreateDefault() => new()
    {
        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
        Header = new ContactHeaderSection
        {
            Badge = "مرکز ارتباط، مشاوره و دسترسی حضوری",
            Title = "همیشه در یک قدمی شما هستیم",
            Description = "از مشاوره انتخاب سایز و متریال تا هماهنگی بازدید حضوری از شوروم و پیگیری سفارش‌ها؛ تیم ورتکس با اشتیاق پاسخگوی شماست."
        },
        Phones = new List<ContactPhoneItem>
        {
            new() { Title = "پشتیبانی و مشاوره خرید", Number = "۰۲۱-۸۸۹۹۰۰۱۱", Raw = "02188990011", Badge = "پاسخگویی سریع", Desc = "شنبه تا پنج‌شنبه از ساعت ۹ الی ۲۱" },
            new() { Title = "پشتیبانی آنلاین و پیام‌رسان‌ها", Number = "۰۹۱۲۳۴۵۶۷۸۹", Raw = "09123456789", Badge = "واتساپ و تلگرام", Desc = "پاسخگویی روزهای کاری و تعطیل" },
            new() { Title = "امور مرسولات و لجستیک", Number = "۰۲۱-۶۶۵۵۴۴۳۳", Raw = "02166554433", Badge = "پیگیری ارسال", Desc = "پیگیری وضعیت بسته‌های پستی و تیپاکس" }
        },
        Emails = new List<ContactEmailItem>
        {
            new() { Title = "پشتیبانی عمومی و مشتریان", Email = "support@vertex-commerce.ir", Desc = "پاسخ ظرف حداکثر ۲ ساعت کاری" },
            new() { Title = "همکاری تجاری و سازمانی", Email = "b2b@vertex-commerce.ir", Desc = "سفارش‌های عمده و همکاری در تامین" }
        },
        WorkingHours = new ContactWorkingHoursSection
        {
            IsOpenNow = true,
            Items = new List<WorkingHourScheduleItem>
            {
                new() { Day = "شنبه تا چهارشنبه", Time = "۰۹:۰۰ الی ۲۱:۰۰", Status = "فعال / یک‌سره" },
                new() { Day = "پنج‌شنبه‌ها", Time = "۰۹:۰۰ الی ۱۸:۰۰", Status = "فعال" },
                new() { Day = "جمعه‌ها و ایام تعطیل رسمی", Time = "۱۱:۰۰ الی ۱۷:۰۰", Status = "فروشگاه حضوری باز است" }
            }
        },
        Location = new ContactLocationSection
        {
            AddressText = "تهران، خیابان ولیعصر، بالاتر از میدان ونک، مجتمع تجاری ورتکس، طبقه ۲، واحد ۲۰۱",
            PostalCode = "۱۹۶۸۶۳۳۱۱۱",
            MapImagePath = "https://images.unsplash.com/photo-1524661135-423995f22d0b?auto=format&fit=crop&w=800&q=80",
            MapTitle = "شوروم مرکزی ورتکس",
            MapSubtitle = "تقاطع ولیعصر و ونک",
            NeshanUrl = "https://neshan.org",
            BaladUrl = "https://balad.ir",
            GoogleMapsUrl = "https://maps.google.com"
        },
        Photos = new List<ContactStorePhotoItem>
        {
            new() { Url = "https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=1200&q=80", Title = "نمای سالن اصلی و ویترین فروشگاه", Tag = "شوروم مرکزی" },
            new() { Url = "https://images.unsplash.com/photo-1555529669-e69e7aa0ba9a?auto=format&fit=crop&w=800&q=80", Title = "میز مشاوره، پرو و ارتباط مستقیم با مشتریان", Tag = "فضای میزبانی" },
            new() { Url = "https://images.unsplash.com/photo-1558769132-cb1aea458c5e?auto=format&fit=crop&w=800&q=80", Title = "کارگاه دوخت، برش و کنترل کیفیت نهایی", Tag = "کارگاه تولیدی" },
            new() { Url = "https://images.unsplash.com/photo-1472851294608-062f824d29cc?auto=format&fit=crop&w=800&q=80", Title = "بخش بسته‌بندی پاکیزه و آماده‌سازی سفارشات", Tag = "لجستیک و ارسال" }
        },
        Socials = new List<ContactSocialItem>
        {
            new() { Platform = "instagram", Name = "اینستاگرام (Instagram)", Handle = "@vertex_boutique", Url = "https://instagram.com", Badge = "کالکشن‌های جدید و استایلینگ", Stats = "+۴۵K دنبال‌کننده", Description = "ویدیوهای معرفی جزئیات پارچه، آنباکسینگ بسته‌ها، تخفیف‌های ۲۴ ساعته استوری و معرفی استایل‌ها." },
            new() { Platform = "telegram", Name = "کانال تلگرام (Telegram)", Handle = "@vertex_official", Url = "https://telegram.org", Badge = "تخفیف‌های ویژه و کدهای هدیه", Stats = "+۱۸K عضو فعال", Description = "اطلاع فوری از جشنواره‌ها، کدهای تخفیف اختصاصی اعضای کانال و پشتیبانی سریع پاسخگویی." },
            new() { Platform = "whatsapp", Name = "واتساپ پشتیبانی (WhatsApp)", Handle = "۰۹۱۲-۳۴۵-۶۷۸۹", Url = "https://whatsapp.com", Badge = "چت و ارسال عکس سایز", Stats = "پاسخگویی آنلاین", Description = "ارسال راهنمای ویدیویی ابعاد و سایز، دریافت عکس‌های بیشتر محصول و ثبت سفارش آسان." },
            new() { Platform = "pinterest", Name = "پینترست (Pinterest)", Handle = "@vertex_moodboard", Url = "https://pinterest.com", Badge = "مودبورد و ترکیب رنگ", Stats = "+۱۰۰ پین اختصاصی", Description = "ایده‌های جذاب ست‌کردن لباس، پالت‌های رنگی الهام‌بخش و ترندهای طراحی پوشاک مینیمال." },
            new() { Platform = "youtube", Name = "یوتیوب (YouTube)", Handle = "@vertex_style", Url = "https://youtube.com", Badge = "ویدیوهای پشت‌صحنه و کیفیت", Stats = "بررسی تخصصی", Description = "ویدیوهای مستند از فرآیند انتخاب متریال، شیوه نگهداری از الیاف طبیعی و راهنمای کامل استایل." },
            new() { Platform = "linkedin", Name = "لینکدین (LinkedIn)", Handle = "vertex-commerce", Url = "https://linkedin.com", Badge = "فرصت‌های شغلی و B2B", Stats = "شبکه رسمی", Description = "اخبار رسمی توسعه برند، فرصت‌های همکاری شغلی، گزارش‌های فصلی و تعامل با شرکای تجاری." }
        },
        Faqs = new List<ContactFaqItem>
        {
            new() { Question = "آیا برای مراجعه حضوری به شوروم و کارگاه نیاز به هماهنگی قبلی است؟", Answer = "خیر، شوروم مرکزی ورتکس همه روزه در ساعات کاری اعلام‌شده (شنبه تا چهارشنبه ۹ الی ۲۱ و پنج‌شنبه‌ها ۹ الی ۱۸) بدون نیاز به هماهنگی قبلی پذیرای شما عزیزان برای لمس متریال و پرو است." },
            new() { Question = "چگونه می‌توانم قبل از خرید مشاوره آنلاین در مورد سایز و رنگ دریافت کنم؟", Answer = "کافیست از طریق شماره پشتیبانی واتساپ (۰۹۱۲۳۴۵۶۷۸۹) پیام دهید تا کارشناسان استایلینگ ما ویدیو و تصاویر دقیق با نور طبیعی را برای شما ارسال کنند." },
            new() { Question = "آیا امکان تحویل حضوری سفارش‌های ثبت‌شده در سایت وجود دارد؟", Answer = "بله، هنگام ثبت سفارش می‌توانید گزینه «تحویل حضوری در شوروم مرکزی» را انتخاب فرمایید و پس از دریافت پیامک آماده‌سازی، در ساعات کاری جهت تحویل کالا تشریف بیاورید." },
            new() { Question = "برای همکاری سازمانی، سفارش‌های عمده یا تامین کالا چگونه اقدام کنیم؟", Answer = "جهت بررسی پیشنهادهای همکاری B2B یا سفارش‌های عمده، می‌توانید مستقیماً به ایمیل b2b@vertex-commerce.ir مکاتبه فرمایید یا با شماره ۰۲۱-۸۸۹۹۰۰۱۱ داخلی ۱۰۴ تماس حاصل فرمایید." }
        },
        UpdatedAt = DateTime.UtcNow
    };
}

public sealed class ContactHeaderSection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class ContactPhoneItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("number")]
    public string Number { get; set; } = string.Empty;

    [BsonElement("raw")]
    public string Raw { get; set; } = string.Empty;

    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
}

public sealed class ContactEmailItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
}

public sealed class ContactWorkingHoursSection
{
    [BsonElement("isOpenNow")]
    public bool IsOpenNow { get; set; } = true;

    [BsonElement("items")]
    public List<WorkingHourScheduleItem> Items { get; set; } = new();
}

public sealed class WorkingHourScheduleItem
{
    [BsonElement("day")]
    public string Day { get; set; } = string.Empty;

    [BsonElement("time")]
    public string Time { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}

public sealed class ContactLocationSection
{
    [BsonElement("addressText")]
    public string AddressText { get; set; } = string.Empty;

    [BsonElement("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [BsonElement("mapImagePath")]
    public string? MapImagePath { get; set; }

    [BsonElement("mapTitle")]
    public string MapTitle { get; set; } = string.Empty;

    [BsonElement("mapSubtitle")]
    public string MapSubtitle { get; set; } = string.Empty;

    [BsonElement("neshanUrl")]
    public string? NeshanUrl { get; set; }

    [BsonElement("baladUrl")]
    public string? BaladUrl { get; set; }

    [BsonElement("googleMapsUrl")]
    public string? GoogleMapsUrl { get; set; }
}

public sealed class ContactStorePhotoItem
{
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("tag")]
    public string Tag { get; set; } = string.Empty;
}

public sealed class ContactSocialItem
{
    [BsonElement("platform")]
    public string Platform { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("handle")]
    public string Handle { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("stats")]
    public string Stats { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("icon")]
    public string Icon { get; set; } = string.Empty;
}

public sealed class ContactFaqItem
{
    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;

    [BsonElement("answer")]
    public string Answer { get; set; } = string.Empty;
}
