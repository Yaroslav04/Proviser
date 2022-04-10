using Proviser.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

HttpClient client = new HttpClient();

var response = await client.GetAsync("https://reyestr.court.gov.ua/Review/76301398"); 

var responseString = await response.Content.ReadAsStringAsync();

var array = responseString.Split("\n");

List<string> list = new List<string>();

bool sw = false;

foreach (var line in array)
{
    if (line.Contains("<body>"))
    {
        sw = true;
    }

    if (line.Contains("</body>"))
    {
        sw = false;
    }

    if (sw)
    {
        list.Add(line);
    }
}

Console.WriteLine(list.Count);

foreach (var item in list)
{
    Console.WriteLine(item);
}


Console.ReadLine();