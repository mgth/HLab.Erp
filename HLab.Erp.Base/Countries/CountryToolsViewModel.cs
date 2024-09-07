using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.ReactiveUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace HLab.Erp.Base.Wpf.Entities.Countries;

public class CountryToolsViewModel : ViewModel
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "tools";
    }


    public CountryToolsViewModel(IDataService data)
    {
        Data = data;
        ImportCommand = ReactiveCommand.CreateFromTask(async () => await LoadCountriesAsync());
    }

    public IDataService Data { get; } 

    public ICommand ImportCommand { get; }

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

    async Task LoadIconAsync(string url, string iconPath)
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