using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{    using H = H<CountryToolsViewModel>;

    public class CountryToolsViewModel : ViewModel
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "tools";
        }


        public CountryToolsViewModel(IDataService data)
        {
            Data = data;
            H.Initialize(this);
        }

        public IDataService Data { get; } 

        public ICommand ImportCommand { get; } = H.Command(c => c
        
            .Action(async e =>
            {
                await e.LoadCountriesAsync();
            })
        
        );

        public async Task LoadCountriesAsync()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
            var contentsUrl = $"https://restcountries.eu/rest/v2/all";

            try
            {
                var contentsJson = await httpClient.GetStringAsync(contentsUrl);

                var contents = (JArray) JsonConvert.DeserializeObject(contentsJson);
                foreach (var country in contents)
                {
                    var name = (string) country["name"];
                    var namefr = (string) country["translations"]["fr"];
                    var c = await Data.FetchOneAsync<Country>(cty =>
                        cty.Name == name || cty.Name == $"{{{name}}}" || cty.Name == namefr);
                    if (c != null)
                    {
                        c.Name = $"{{{name}}}";
                        c.Iso = int.Parse((string) country["numericCode"]);
                        c.IsoA2 = (string) country["alpha2Code"];
                        c.IsoA3 = (string) country["alpha3Code"];
                        c.IconPath = $"Icon/Country/Flag/{c.IsoA3}";
                        await Data.SaveAsync(c);

                        var flagUrl = (string)country["flag"];

                        await LoadIconAsync(flagUrl, c.IconPath);
                    }
                    else
                    {

                    }
                }
            }
            catch(HttpRequestException ex)
            {

            }
        }

        private async Task LoadIconAsync(string url, string iconPath)
        {
           var icon = await Data.FetchOneAsync<Icon>(i =>
                    i.Path==iconPath);
           if (icon != null) return;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("HLab", "1"));
            try
            {

                var svg = await httpClient.GetStringAsync(url);
                await Data.AddAsync<Icon>(i =>
                {
                    i.Path = iconPath;
                    i.SourceSvg = svg;
                });
            }
            catch(HttpRequestException ex)
            {

            }
        }

    }
}