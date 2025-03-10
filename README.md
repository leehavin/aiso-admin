# AiSo Admin

## 项目介绍
AiSo Admin 是一个基于 AiUo 框架开发的现代化后台管理系统，采用.NET 8.0构建。本项目提供了完整的权限管理、组织架构、数据字典等基础功能，可以作为企业级后台管理系统的开发基础框架。

## 技术栈
- 开发框架：.NET 8.0
- 基础框架：AiUo Framework
- 数据访问：SqlSugar ORM
- 缓存服务：Redis
- 消息队列：RabbitMQ
- 日志服务：Serilog
- 认证授权：JWT + HCaptcha
- API文档：Swagger/OpenAPI
- 服务发现：Nacos
- 监控度量：AppMetrics

## 项目结构
```
AiSo.Admin/
├── src/
│   ├── AiSo.Admin.Repository/    # 数据访问层
│   ├── AiSo.Admin.Service/       # 业务逻辑层
│   └── AiSo.Admin.WebApi/        # Web API层
```

## 主要功能
- 用户管理：用户信息管理、角色分配、岗位分配等
- 角色管理：角色信息管理、菜单权限分配、数据权限分配等
- 菜单管理：菜单信息管理、按钮权限管理等
- 部门管理：部门组织结构管理
- 岗位管理：岗位信息管理
- 字典管理：系统中各种枚举类型的维护
- 操作日志：系统操作日志记录和查询
- 服务监控：服务器状态监控

## 特色优势
- 基于AiUo框架，提供丰富的企业级功能组件
- 完善的权限管理体系
- 支持多语言国际化
- 集成数据缓存机制
- 分布式架构支持
- 完整的日志追踪系统
- 灵活的数据权限控制

## 快速开始

### 环境要求
- .NET 8.0 SDK
- Redis
- RabbitMQ
- SQL Server/MySQL/PostgreSQL（任选其一）

### 安装步骤
1. 克隆代码库
```bash
git clone https://github.com/your-username/aiso-admin.git
```

2. 修改配置
在 `src/AiSo.Admin.WebApi/appsettings.json` 中配置数据库连接、Redis连接等信息

3. 运行项目
```bash
cd src/AiSo.Admin.WebApi
dotnet run
```

4. 访问接口文档
```
http://localhost:5000/swagger
```

## 贡献指南
欢迎提交Issue和Pull Request

## 许可证
MIT License

## 联系我们
- 项目地址：[GitHub Repository](https://github.com/your-username/aiso-admin)
- 问题反馈：请提交Issue