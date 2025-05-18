
### 1. Убедитесь, что у вас установлены:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/compose/install/)


### 2. Поднять БД в Docker

В проекте уже есть `docker-compose.yml`, нужно только выполнить команду:

```bash
docker-compose up -d
```

Если нужно подключиться к базе данных, например через DataGrip, в файле `appsettings.Development.json` есть строка подключения со всеми нужными параметрами

### 3. Выполнить миграции

Если EF CLI не установлен:

```bash
dotnet tool install --global dotnet-ef
```

Применить миграции:
```bash
dotnet ef database update
```

### 4. Первый запуск проекта

Проект можно запустить из IDE или командой:
```bash
dotnet run
```

Swager находится по url:
```bash
http://localhost:port/swagger/index.html
```

При первом запуске автоматически создается админ с логином и паролем `admin`, от его имени можно залогиниться

Для смены пользователей есть эндпоинты:
- `/login`
- `/logout`



