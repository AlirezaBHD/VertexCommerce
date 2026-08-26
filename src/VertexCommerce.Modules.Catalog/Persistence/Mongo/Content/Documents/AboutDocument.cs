using MongoDB.Bson.Serialization.Attributes;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

public sealed class AboutDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [BsonElement("hero")]
    public AboutHeroSection Hero { get; set; } = new();

    [BsonElement("commitments")]
    public AboutCommitmentsSection Commitments { get; set; } = new();

    [BsonElement("quality")]
    public AboutQualitySection Quality { get; set; } = new();

    [BsonElement("process")]
    public AboutProcessSection Process { get; set; } = new();

    [BsonElement("story")]
    public AboutStorySection Story { get; set; } = new();

    [BsonElement("cta")]
    public AboutCtaSection Cta { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    public static AboutDocument CreateDefault() => new()
    {
        Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
        Hero = new AboutHeroSection
        {
            Badge = "درباره ما",
            Title = "با دقت انتخاب می‌کنیم، با خیال راحت همراهتان هستیم",
            Subtitle = "ما این فروشگاه را با یک هدف ساده ساخته‌ایم: ارائه محصولاتی که انتخابشان برای شما آسان باشد و تجربه خریدی که از ابتدا تا انتها قابل اعتماد و خوشایند بماند.",
            ButtonText = "مشاهده تعهدها و داستان ما",
            ImagePath = "/linen-shirt.jpg",
            ShowCat = true,
            CatImagePath = "/cat.png"
        },
        Commitments = new AboutCommitmentsSection
        {
            Badge = "اصول بنیادین ما",
            Title = "تعهدهای کیفی بدون مصالحه",
            Subtitle = "به جای اعداد و آمار تجاری، پای تعهداتی ایستاده‌ایم که حس اعتماد و اصالت را برای شما ملموس می‌کنند.",
            Items = new List<AboutCommitmentItem>
            {
                new() { Title = "ارسال ۲۴ ساعته", Description = "سفارش‌های ثبت‌شده بدون اتلاف وقت و با نهایت دقت، ظرف یک شبانه‌روز آماده‌سازی و تحویل شبکه ارسال می‌شوند.", Badge = "سرعت و دقت", Icon = "fast-delivery" },
                new() { Title = "انتخاب دقیق و اصیل", Description = "تک‌تک محصولات از نظر کیفیت ساخت، کارایی و هماهنگی با نیازهای واقعی شما با وسواس و دقت بررسی و تایید می‌شوند.", Badge = "کیفیت بی‌واسطه", Icon = "quality" },
                new() { Title = "تعویض بدون سوال", Description = "اگر محصول دقیقاً با انتظار شما همخوان نبود، بی هیچ پرسش و معطلی تعویض یا مرجوع می‌شود.", Badge = "آرامش خاطر", Icon = "return" }
            }
        },
        Quality = new AboutQualitySection
        {
            Badge = "نگرش ما به کیفیت",
            Title = "کیفیت در جزئیات، شفافیت در انتخاب",
            Paragraphs = new List<string>
            {
                "پشت هر محصولی که در این فروشگاه می‌بینید، زمانی برای انتخاب، بررسی و آماده‌سازی صرف شده است. ما تلاش می‌کنیم محصولاتی را ارائه کنیم که خرید از آن‌ها برای شما تجربه‌ای ساده، ماندگار و مطمئن باشد.",
                "برای ما، فروش فقط ثبت یک سفارش نیست؛ بلکه فرصتی برای ساختن یک ارتباط محترمانه و بلندمدت با شماست. به همین دلیل در معرفی، پاسخگویی، بسته‌بندی و ارسال، تجربه‌ای منظم ارائه می‌دهیم."
            },
            Features = new List<AboutQualityFeatureItem>
            {
                new() { Title = "اطلاعات شفاف", Description = "معرفی دقیق و بدون اغراق" },
                new() { Title = "کیفیت پایدار", Description = "کارایی و ماندگاری واقعی" }
            },
            ImagePath = "https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=1000&q=80",
            ImageBadgeTitle = "دقت در جزئیات",
            ImageBadgeSubtitle = "احترام به انتخاب شما و تضمین آرامش خاطر در تمام مراحل خرید"
        },
        Process = new AboutProcessSection
        {
            Badge = "فرآیند کار ما",
            Title = "تجربه خرید، ساده و روشن",
            Subtitle = "از لحظه انتخاب تا زمانی که بسته با نهایت نظم و دقت به دست شما می‌رسد.",
            Steps = new List<AboutProcessStepItem>
            {
                new() { Title = "انتخاب آگاهانه", Description = "بررسی ویژگی‌ها، جزئیات و اطلاعات کامل محصولات برای خریدی شفاف و مطمئن.", Icon = "quality" },
                new() { Title = "ثبت مطمئن سفارش", Description = "طراحی فرآیند خرید ساده، سریع و امن بدون پیچیدگی و سردرگمی.", Icon = "authenticity" },
                new() { Title = "بسته‌بندی پاک و ایمن", Description = "قرارگیری کالاها در بسته‌بندی‌های شکیل و اصولی برای حفظ سلامت کامل سفارش.", Icon = "package" },
                new() { Title = "تحویل مستقیم به دست شما", Description = "ارسال منظم و پیگیری لحظه‌ای تا رسیدن سفارش به مقصد در کوتاه‌ترین زمان.", Icon = "shipping" }
            }
        },
        Story = new AboutStorySection
        {
            Badge = "داستان ما",
            Title = "داستان این گوشه‌ی دنج",
            Paragraphs = new List<string>
            {
                "این فروشگاه با یک ایده ساده شکل گرفت: ایجاد فضایی که انتخاب کردن در آن راحت باشد، مشتری بداند با چه تیمی روبه‌رو است و از لحظه ورود تا دریافت سفارش، احساس احترام و آرامش داشته باشد.",
                "اینجا خبری از ساختارهای خشک و پاسخ‌های ماشینی نیست؛ پشت این فروشگاه افرادی هستند که به تک‌تک سفارش‌ها و نظرات شما اهمیت می‌دهند و با دقت فرایند خرید را همراهی می‌کنند تا تجربه‌ای خوشایند در خاطرتان بماند."
            },
            ImagePath = "https://images.unsplash.com/photo-1556742049-0a67c5574f73?auto=format&fit=crop&w=1000&q=80",
            ImageBadge = "پاسخگویی دقیق و همراهی مستقیم",
            SupportText = "پاسخگویی مستقیم و همراهی صمیمانه در تمام مراحل خرید"
        },
        Cta = new AboutCtaSection
        {
            Title = "انتخاب بعدی شما می‌تواند از همین‌جا شروع شود",
            Subtitle = "محصولات فروشگاه را ببینید و با خیال راحت و اطمینان کامل خرید کنید.",
            ButtonText = "مشاهده و انتخاب محصولات",
            ButtonLink = "/products"
        },
        UpdatedAt = DateTime.UtcNow
    };
}

public sealed class AboutHeroSection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("buttonText")]
    public string ButtonText { get; set; } = string.Empty;

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("showCat")]
    public bool ShowCat { get; set; } = true;

    [BsonElement("catImagePath")]
    public string? CatImagePath { get; set; }
}

public sealed class AboutCommitmentsSection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<AboutCommitmentItem> Items { get; set; } = new();
}

public sealed class AboutCommitmentItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("icon")]
    public string Icon { get; set; } = string.Empty;
}

public sealed class AboutQualitySection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("paragraphs")]
    public List<string> Paragraphs { get; set; } = new();

    [BsonElement("features")]
    public List<AboutQualityFeatureItem> Features { get; set; } = new();

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("imageBadgeTitle")]
    public string? ImageBadgeTitle { get; set; }

    [BsonElement("imageBadgeSubtitle")]
    public string? ImageBadgeSubtitle { get; set; }
}

public sealed class AboutQualityFeatureItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class AboutProcessSection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("steps")]
    public List<AboutProcessStepItem> Steps { get; set; } = new();
}

public sealed class AboutProcessStepItem
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("icon")]
    public string Icon { get; set; } = string.Empty;
}

public sealed class AboutStorySection
{
    [BsonElement("badge")]
    public string Badge { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("paragraphs")]
    public List<string> Paragraphs { get; set; } = new();

    [BsonElement("imagePath")]
    public string? ImagePath { get; set; }

    [BsonElement("imageBadge")]
    public string? ImageBadge { get; set; }

    [BsonElement("supportText")]
    public string? SupportText { get; set; }
}

public sealed class AboutCtaSection
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("buttonText")]
    public string ButtonText { get; set; } = string.Empty;

    [BsonElement("buttonLink")]
    public string ButtonLink { get; set; } = string.Empty;
}
