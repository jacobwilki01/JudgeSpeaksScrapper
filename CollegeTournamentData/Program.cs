using CollegeTournamentData;
using System.Text.Json;
using IronXL;

List<KeyValuePair<string, string>> Ids = new() 
{ 
    new("24235", "NU 22"),
    new("24297", "UK 22"),
    new("24531", "Gonzaga 22"),
    new("24371", "Wake 22"),
    new("25222", "Georgetown 22"),
    new("26034", "Texas 22"),
    new("26689", "ADA 22"),
    new("26828", "NDT 22"),
    new("26327", "CEDA 22"),
    new("27613", "NU 23"),
};

Uri baseAddress = new Uri("https://www.tabroom.com");
HttpClient client = new HttpClient();
client.BaseAddress = baseAddress;

foreach (KeyValuePair<string, string> Id in Ids)
{
    HttpResponseMessage response = await client.GetAsync($"/api/download_data?tourn_id={Id.Key}");

    Stream content = await response.Content.ReadAsStreamAsync();

    FileStream stream = new FileStream($"{Id.Value}.json", FileMode.Create, FileAccess.Write, FileShare.None, 256000, true);

    await content.CopyToAsync(stream);
}

List<Tournament> tournaments = new();

foreach (KeyValuePair<string, string> Id in Ids)
{
    string filepath = Id.Value + ".json";
    string jsonString = File.ReadAllText(filepath);

    Tournament tournament = JsonSerializer.Deserialize<Tournament>(jsonString)!;
    tournaments.Add(tournament);
}

List<Judge> judges = new();

foreach (Tournament tournament in tournaments)
{
    Console.WriteLine("Beginning " + tournament.name);

    List<Category> openCategories = tournament.categories.Where(category => !category.name!.Contains("PNW") && !category.name!.Contains("Observer")).ToList();

    foreach (Category category in openCategories)
    {
        List<Event> openEvents = category.events.Where(@event => @event.name!.ToLower().Contains("open")).ToList();

        foreach (Event @event in openEvents)
        {
            List<Round> prelimRounds = @event.rounds.Where(round => round.protocol_name!.Contains("Prelim")).ToList();

            foreach (Round round in prelimRounds)
            {
                foreach (Section section in round.sections)
                {
                    foreach (Ballot ballot in section.ballots)
                    {
                        if (ballot.judge_first == null || ballot.judge_last == null)
                            continue;

                        Judge? judge = judges.Find(jdg => jdg.FirstName.Equals(ballot.judge_first) && jdg.LastName.Equals(ballot.judge_last.Replace(" - ONLINE", string.Empty).Replace("-ONLINE", string.Empty).Replace("- ONL", string.Empty)));

                        if (judge == null)
                        {
                            judge = new Judge()
                            {
                                FirstName = ballot.judge_first!,
                                LastName = ballot.judge_last!.Replace(" - ONLINE", string.Empty).Replace("-ONLINE", string.Empty).Replace("- ONL", string.Empty)
                            };

                            judges.Add(judge);
                        }

                        foreach (Score score in ballot.scores)
                        {
                            if (score.tag!.Equals("point"))
                                judge.Speaks.Add((float)score.value!);
                        }
                    }
                }
            }
        }
    }

    Console.WriteLine("Finishing " + tournament.name);
}

WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);
var worksheet = workbook.CreateWorkSheet("Judge Speak Averages");
worksheet["A1"].Value = "First Name";
worksheet["B1"].Value = "Last Name";
worksheet["C1"].Value = "Avg Speaks";
worksheet["D1"].Value = "StDev Speaks";
worksheet["E1"].Value = "Min Speak";
worksheet["F1"].Value = "Max Speak";

int row = 1;
foreach (Judge judge in judges)
{
    Console.WriteLine("Beginning " + judge.FirstName + " " + judge.LastName);
    worksheet.SetCellValue(row, 0, judge.FirstName);
    worksheet.SetCellValue(row, 1, judge.LastName);
    worksheet.SetCellValue(row, 2, judge.GetAvg());
    worksheet.SetCellValue(row, 3, judge.GetStDev());
    worksheet.SetCellValue(row, 4, judge.GetMin());
    worksheet.SetCellValue(row, 5, judge.GetMax());
    Console.WriteLine("Finishing " + judge.FirstName + judge.LastName);
    row++;
}

workbook.SaveAs("JudgeSpeaks.xlsx");