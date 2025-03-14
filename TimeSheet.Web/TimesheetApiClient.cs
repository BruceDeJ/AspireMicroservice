using System.Net.Http.Headers;

namespace TimeSheet.Web;

public class TimesheetApiClient(HttpClient httpClient)
{
    public async Task<List<TimeSheetEntry>> GetTimeSheetEntries(string loginGuid, CancellationToken cancellationToken = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginGuid);

        var response = await httpClient.GetAsync("/TimeSheetEntry");

        var timesheetEntries = await response.Content.ReadFromJsonAsync<List<TimeSheetEntry>>() ?? new List<TimeSheetEntry>();
        
        return timesheetEntries;
    }

    public async Task<bool> CreateTimeSheetEntry(string loginGuid, int durationInMinutes, string description, CancellationToken cancellationToken = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginGuid);

        var response = await httpClient.PostAsJsonAsync("/TimeSheetEntry", new
        {
            DurationInMinutes = durationInMinutes,
            Description = description
        });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTimeSheetEntry(string loginGuid, int timesheetEntryId, CancellationToken cancellationToken = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginGuid);

        var response = await httpClient.DeleteAsync($"/TimeSheetEntry/{timesheetEntryId}");

        return response.IsSuccessStatusCode ? true : false;
    }

    public class TimeSheetEntry
    {
        public int Id { get; set; }
        public int DurationInMinutes { get; set; }
        public string Description { get; set; }
        public DateTime DateLogged { get; set; }
        public string LoggedBy { get; set; }
    }
}