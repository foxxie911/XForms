namespace XForms.Data.Dtos;

public class GridDataDto
{
    public string? SearchText { get; set; } = null;
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}