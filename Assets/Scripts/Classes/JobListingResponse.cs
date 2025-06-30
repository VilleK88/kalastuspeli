[System.Serializable]
public class JobListingResponse
{
    public int total;
    public int page;
    public int per_page;
    public int pages;
    public JobListing[] results;
}