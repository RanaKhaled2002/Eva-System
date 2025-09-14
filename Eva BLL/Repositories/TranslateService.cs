using Eva_BLL.Interfaces;
using GTranslate.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eva_BLL.Repositories
{
    public class TranslateService : ITranslateService
    {
        private readonly ITranslator _translator;

        public TranslateService()
        {
            _translator = new GoogleTranslator();
        }

        public async Task<string> TranslateTextAsync(string text, string LanguageCode)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(LanguageCode))
                return null;

            var result = await _translator.TranslateAsync(text, LanguageCode);
            return result?.Translation;
        }
    }
}
