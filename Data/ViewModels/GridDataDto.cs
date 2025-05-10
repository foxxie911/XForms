namespace XForms.Data.ViewModels;

public class GridDataDto
{
    public string? SearchText { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; } = 10;
}