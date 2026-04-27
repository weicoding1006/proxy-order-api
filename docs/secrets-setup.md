# 密鑰設定說明

## JWT Key

`appsettings.json` 中的 `Jwt:Key` 為佔位字串，**真實密鑰不存在 repo 中**。

### 開發環境設定

clone 專案後執行以下指令，在本機設定 JWT Key：

```bash
dotnet user-secrets set "Jwt:Key" "your-random-secret-key-at-least-32-chars" --project OrderSystem.Api
```

確認是否設定成功：

```bash
dotnet user-secrets list --project OrderSystem.Api
```

Key 會存放在本機的：
```
%APPDATA%\Microsoft\UserSecrets\c56e939b-9867-4df9-ad11-63e5121c05bb\secrets.json
```

### 正式環境設定

透過環境變數注入，不需要修改任何檔案：

```bash
# Linux / Docker
export Jwt__Key="正式環境的超長隨機密鑰"

# Windows Server
setx Jwt__Key "正式環境的超長隨機密鑰" /M
```

> 注意：環境變數使用 `__`（雙底線）對應設定檔的 `:` 分隔符號
