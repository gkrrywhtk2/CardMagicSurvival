using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine;

public class Loadlocale : MonoBehaviour
{
   public void LoadLocale(string languageIdentifier) {
    LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);
    for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++) {
    	Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
        LocaleIdentifier anIdentifier = aLocale.Identifier;
        if(anIdentifier == localeCode) {
        	LocalizationSettings.SelectedLocale = aLocale;
        }
    }
}
}
