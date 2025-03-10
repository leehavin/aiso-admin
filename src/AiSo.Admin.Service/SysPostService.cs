namespace AiSo.Admin.Service;

/// <summary>
/// 岗位服务
/// </summary>
/// <param name="serviceProvider"></param>
public class SysPostService(IServiceProvider serviceProvider)
    : ApplicationService<SysPost, Guid, SysPost, SysPost>(serviceProvider)
{
    /// <summary>
    /// pagingSearchInput
    /// </summary>
    /// <param name="pagingSearchInput"></param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysPost> pagingSearchInput)
    {
        var query = Repository.Select
                .WhereIf(!string.IsNullOrWhiteSpace(pagingSearchInput.Search?.Name),
                    a => a.Name.Contains(pagingSearchInput.Search.Name))
                .OrderBy(w => w.Number)
                .Select(w => new
                {
                    w.Number,
                    w.Code,
                    w.Name,
                    //State = w.State == StateEnum.正常 ? "正常" : "停用",
                    w.State,
                    //State=w.State.ToString(),
                    w.LastModificationTime,
                    w.CreationTime,
                    w.Id,
                })
            ;

        var result = await Repository.AsPagingViewAsync(query, pagingSearchInput);
        //覆盖值
        result
            .FormatValue(query, w => w.CreationTime, (oldValue) => oldValue.ToString("yyyy-MM-dd"))
            .FormatValue(query, w => w.LastModificationTime, (oldValue) => oldValue?.ToString("yyyy-MM-dd"))
            ;

        return result;
    }

    /// <summary>
    /// 查询表单数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override async Task<Dictionary<string, object?>> FindFormAsync(Guid id)
    {
        var res = new Dictionary<string, object?>();
        var form = await Repository.FindByIdAsync(id);
        form = form.NullSafe();

        if (id == Guid.Empty)
        {
            var maxNum = await Repository.Select.MaxAsync(w => w.Number);
            form.Number = (maxNum ?? 0) + 1;
        }

        res[nameof(id)] = id == Guid.Empty ? "" : id;
        res[nameof(form)] = form;
        return res;
    }
}