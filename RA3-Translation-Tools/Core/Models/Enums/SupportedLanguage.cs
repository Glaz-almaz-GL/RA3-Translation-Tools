using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RA3_Translation_Tools.Core.Models.Enums
{
    public enum SupportedLanguage
    {
        [Description("Авто")]
        Auto,
        [Description("Английский")]
        English,
        [Description("Русский")]
        Russian,
        [Description("Французский")]
        French,
        [Description("Немецкий")]
        German,
        [Description("Испанский")]
        Spanish,
        [Description("Итальянский")]
        Italian,
        [Description("Португальский")]
        Portuguese,
        [Description("Китайский")]
        Chinese,
        [Description("Японский")]
        Japanese,
        [Description("Корейский")]
        Korean,
        [Description("Арабский")]
        Arabic,
        [Description("Неизвестный")]
        Unknown,
        [Description("Бенгальский")]
        Bengali,
        [Description("Хинди")]
        Hindi,
        [Description("Тамильский")]
        Tamil,
        [Description("Телугу")]
        Telugu,
        [Description("Малаялам")]
        Malayalam,
        [Description("Каннада")]
        Kannada,
        [Description("Гуджарати")]
        Gujarati,
        [Description("Панджаби")]
        Punjabi,
        [Description("Маратхи")]
        Marathi,
        [Description("Тайский")]
        Thai,
        [Description("Вьетнамский")]
        Vietnamese,
        [Description("Индонезийский")]
        Indonesian,
        [Description("Малайский")]
        Malay,
        [Description("Турецкий")]
        Turkish,
        [Description("Польский")]
        Polish,
        [Description("Голландский")]
        Dutch,
        [Description("Шведский")]
        Swedish,
        [Description("Финский")]
        Finnish,
        [Description("Датский")]
        Danish,
        [Description("Норвежский")]
        Norwegian,
        [Description("Иврит")]
        Hebrew,
        [Description("Персидский")]
        Persian,
        [Description("Урду")]
        Urdu,
        [Description("Хорватский")]
        Croatian,
        [Description("Сербский")]
        Serbian,
        [Description("Болгарский")]
        Bulgarian,
        [Description("Румынский")]
        Romanian,
        [Description("Украинский")]
        Ukrainian,
        [Description("Чешский")]
        Czech,
        [Description("Словацкий")]
        Slovak,
        [Description("Словенский")]
        Slovenian,
        [Description("Венгерский")]
        Hungarian,
        [Description("Эстонский")]
        Estonian,
        [Description("Латышский")]
        Latvian,
        [Description("Литовский")]
        Lithuanian,
        [Description("Исландский")]
        Icelandic,
        [Description("Греческий")]
        Greek,
        [Description("Тагальский")]
        Tagalog,
        [Description("Суахили")]
        Swahili,
        [Description("Африкаанс")]
        Afrikaans,
        [Description("Зулу")]
        Zulu,
        [Description("Кхмерский")]
        Khmer,
        [Description("Лаосский")]
        Lao,
        [Description("Мьянманский")]
        Myanmar,
        [Description("Монгольский")]
        Mongolian,
        [Description("Непальский")]
        Nepali,
        [Description("Сингальский")]
        Sinhala
    }

    public static class SupportedLanguageExtensions
    {
        // Теперь возвращаем список LanguageItem
        public static LanguageItem[] ItemsSource { get; } = GetLanguageItems();

        private static LanguageItem[] GetLanguageItems()
        {
            SupportedLanguage[] values = System.Enum.GetValues<SupportedLanguage>();
            LanguageItem[] items = new LanguageItem[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                items[i] = new LanguageItem(values[i], GetDescription(values[i]));
            }

            return items;
        }

        private static string GetDescription(SupportedLanguage language)
        {
            FieldInfo? field = typeof(SupportedLanguage).GetField(language.ToString());
            return field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute attr
                ? attr.Description
                : language.ToString();
        }
    }
}