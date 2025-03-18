namespace HotelProject.Domain.Model.Commons ;

public class PageResult<TResult>
{
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public List<TResult> Data { get; set; }
    public int AmountInPage  => Data.Count;
}