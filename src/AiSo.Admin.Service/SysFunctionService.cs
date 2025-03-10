namespace AiSo.Admin.Service;

/// <summary>
/// 功能服务
/// </summary>
public class SysFunctionService(IServiceProvider serviceProvider)
    : ApplicationService<SysFunction, Guid, SysFunction, SysFunction>(serviceProvider)
{
    /// <summary>
    /// pagingSearchInput
    /// </summary>
    /// <param name="pagingSearchInput"></param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysFunction> pagingSearchInput)
    {
        var query = Repository.Select
                .WhereIf(!string.IsNullOrWhiteSpace(pagingSearchInput.Search?.Name),
                    a => a.Name != null && a.Name.Contains(pagingSearchInput.Search!.Name))
                .OrderBy(w => w.Number)
                .Select(w => new
                {
                    w.Number,
                    w.Name,
                    w.ByName,
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