namespace AiSo.Admin.Service;

public class SysServerService : ISingletonSelfDependency
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpClientFactory _httpClientFactory;
    private MemoryMetrics _currentMemoryMetrics = new MemoryMetrics();
    private List<NetWorkMetrics> _currentNetWorkMetrics = new List<NetWorkMetrics>();
    private readonly Queue<MemoryMetrics> _memoryMetrics;
    private readonly Queue<List<NetWorkMetrics>> _netWorkMetrics;

    private readonly int _count = 10 * 60;

    public SysServerService(IWebHostEnvironment webHostEnvironment,
        IHttpClientFactory httpClientFactory)
    {
        _webHostEnvironment = webHostEnvironment;
        _httpClientFactory = httpClientFactory;
        _memoryMetrics = new Queue<MemoryMetrics>(_count);
        _netWorkMetrics = new Queue<List<NetWorkMetrics>>(_count);
    }

    /// <summary>
    /// 初始化内存和网络计数器
    /// </summary>
    /// <returns></returns>
    public async Task InitAsync()
    {
        await Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var startTimestamp = Stopwatch.GetTimestamp();
                if (_memoryMetrics.Count >= _count)
                {
                    _memoryMetrics.Dequeue();
                }

                if (_netWorkMetrics.Count >= _count)
                {
                    _netWorkMetrics.Dequeue();
                }

                _currentMemoryMetrics = ComputerUtil.GetComputerInfo();
                _currentNetWorkMetrics = ComputerUtil.GetNetWorkMetrics();
                if (DateTime.Now.Second == 0)
                {
                    _memoryMetrics.Enqueue(_currentMemoryMetrics);
                    _netWorkMetrics.Enqueue(_currentNetWorkMetrics);
                }

                var timeSpan = Stopwatch.GetElapsedTime(startTimestamp);

                var time = TimeSpan.FromSeconds(1) - timeSpan;

                if (time > TimeSpan.Zero)
                {
                    await Task.Delay(time);
                }
            }
        }, TaskCreationOptions.LongRunning);
    }


    /// <summary>
    /// 获取服务器配置信息
    /// </summary>
    /// <returns></returns>
    public async Task<dynamic> GetServerBaseAsync()
    {
        return new
        {
            HostName = Environment.MachineName, // 主机名称
            SystemOs = ComputerUtil.GetOSInfo(), //RuntimeInformation.OSDescription, // 操作系统
            OsArchitecture = Environment.OSVersion.Platform.ToString() + " " + RuntimeInformation.OSArchitecture.ToString(), // 系统架构
            ProcessorCount = Environment.ProcessorCount + " 核", // CPU核心数
            SysRunTime = ComputerUtil.GetRunTime(), // 系统运行时间
            RemoteIp = await GetIpFromOnlineAsync(), // 外网地址
            LocalIp = App.HttpContext?.Connection?.LocalIpAddress.MapToIPv4().ToString(), // 本地地址
            RuntimeInformation.FrameworkDescription, // NET框架
            Environment = _webHostEnvironment.IsDevelopment() ? "Development" : "Production",
            Wwwroot = _webHostEnvironment.WebRootPath, // 网站根目录
            Stage = _webHostEnvironment.IsStaging() ? "Stage环境" : "非Stage环境", // 是否Stage环境
        };
    }

    /// <summary>
    /// 获取服务器使用信息
    /// </summary>
    /// <returns></returns>
    public dynamic GetServerUsed()
    {
        var programStartTime = Process.GetCurrentProcess().StartTime;
        var totalMilliseconds = (DateTime.Now - programStartTime).TotalMilliseconds.ToString();
        var ts = totalMilliseconds.Contains('.') ? totalMilliseconds.Split('.')[0] : totalMilliseconds;
        var programRunTime = TimeSpan.FromMilliseconds(ts.ToLong()).ToDayHourMinutesSeconds();

        var memoryMetrics = _currentMemoryMetrics;
        var netWorkMetrics = _currentNetWorkMetrics;
        return new
        {
            memoryMetrics.FreeRam, // 空闲内存
            memoryMetrics.UsedRam, // 已用内存
            memoryMetrics.TotalRam, // 总内存
            memoryMetrics.RamRate, // 内存使用率
            memoryMetrics.CpuRate, // Cpu使用率
            netWorkMetrics, // 网络使用信息
            StartTime = programStartTime.ToString("yyyy-MM-dd HH:mm:ss"), // 服务启动时间
            RunTime = programRunTime, // 服务运行时间
        };
    }

    public List<MemoryMetrics> GetHistoryMemoryMetrics() => [.. _memoryMetrics];

    public Dictionary<string, List<NetWorkMetrics>> GetHistoryNetWorkMetrics()
    {
        return _netWorkMetrics
            .SelectMany(w => w.ToArray())
            .GroupBy(w => w.Adapter)
            .ToDictionary(w => w.Key, w => w.OrderBy(o => o.DateTime).ToList());
    }

    /// <summary>
    /// 获取服务器磁盘信息
    /// </summary>
    /// <returns></returns>
    public List<DiskInfo> GetServerDisk()
    {
        return ComputerUtil.GetDiskInfos();
    }

    /// <summary>
    /// 获取框架主要程序集
    /// </summary>
    /// <returns></returns>
    public dynamic GetAssemblyList()
    {
        var autoMapAssembly = typeof(AutoMapper.AutoMapAttribute).Assembly.GetName();
        var hzyCoreAssembly = typeof(CoreStartup).Assembly.GetName();
        var hzyDependencyInjectionAssembly = typeof(DynamicProxyClass).Assembly.GetName();
        var hzyDictSourceGeneratorAssembly = typeof(Framework.Dict.SourceGenerator.CodeInfo).Assembly.GetName();
        var hzyDynamicApiControllerAssembly = typeof(DynamicApiControllerAttribute).Assembly.GetName();
        var hzyRepositoryAssembly = typeof(IBaseDbContext).Assembly.GetName();
        var yitIdAssembly = typeof(YitIdHelper).Assembly.GetName();
        var miniExcelAssembly = typeof(MiniExcel).Assembly.GetName();
        var npoiAssembly = typeof(NPOI.CoreProperties).Assembly.GetName();
        var jsonAssembly = typeof(NewtonsoftJsonMvcCoreBuilderExtensions).Assembly.GetName();
        var quartzAssembly = typeof(CronScheduleBuilder).Assembly.GetName();
        var rougamoAssembly = typeof(Rougamo.AsyncMo).Assembly.GetName();
        var serilogAssembly = typeof(Serilog.Log).Assembly.GetName();
        var flurlAssembly = typeof(Flurl.Url).Assembly.GetName();
        var memoryMQAssembly = typeof(Zyx.MemoryMQ.Attrbutes.SubscribeAttribute).Assembly.GetName();


        return new[]
        {
            new { autoMapAssembly.Name, autoMapAssembly.Version },
            new { hzyCoreAssembly.Name, hzyCoreAssembly.Version },
            new { hzyDependencyInjectionAssembly.Name, hzyDependencyInjectionAssembly.Version },
            new { hzyDictSourceGeneratorAssembly.Name, hzyDictSourceGeneratorAssembly.Version },
            new { hzyDynamicApiControllerAssembly.Name, hzyDynamicApiControllerAssembly.Version },
            new { hzyRepositoryAssembly.Name, hzyRepositoryAssembly.Version },
            new { yitIdAssembly.Name, yitIdAssembly.Version },
            new { miniExcelAssembly.Name, miniExcelAssembly.Version },
            new { npoiAssembly.Name, npoiAssembly.Version },
            new { jsonAssembly.Name, jsonAssembly.Version },
            new { quartzAssembly.Name, quartzAssembly.Version },
            new { rougamoAssembly.Name, rougamoAssembly.Version },
            new { serilogAssembly.Name, serilogAssembly.Version },
            new { flurlAssembly.Name, flurlAssembly.Version },
            new { memoryMQAssembly.Name, memoryMQAssembly.Version },
        };
    }


    /// <summary>
    /// 获取外网IP地址
    /// </summary>
    /// <returns></returns>
    private Task<string> GetIpFromOnlineAsync()
    {
        return Task.FromResult("unknow");
        //try
        //{
        //    var client = _httpClientFactory.CreateClient("NoCer");
        //    var resp = await client.GetFromJsonAsync<IpCnResp>("https://www.ip.cn/api/index?ip&type=0");
        //    return resp.Ip + " " + resp.Address;
        //}
        //catch
        //{
        //    return "unknow";
        //}
    }
}

/// <summary>
/// IP信息
/// </summary>
public class IpCnResp
{
    public string Ip { get; set; }

    public string Address { get; set; }
}