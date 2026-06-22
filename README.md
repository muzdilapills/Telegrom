# Telegrom by Muzdilapills

## Описание Telegrom
### Что такое Telegrom?
Telegrom - Это мессенджер с открытым исходным кодом, с оптимизацией и многим другим.



# Структура проекта Telegrom

### Models (Модели данных)

- **User.cs** - Пользователь (никнейм, email, пароль, аватар)
- **Chat.cs** - Групповой чат (название, создатель, участники, сообщения)
- **Channel.cs** - Канал (название, создатель, подписчики, сообщения)
- **Message.cs** - Сообщение (отправитель, текст, время, вложение)
- **PrivateChat.cs** - Личный чат (два пользователя, сообщения)
- **Contact.cs** - Контакт (владелец, контакт, кастомное имя)
- **FavoriteMessage.cs** - Избранное (пользователь, текст, время)
- **UserSettings.cs** - Настройки (тема, приватность, аватар, обои)
- **BannedUser.cs** - Забаненный пользователь (ник, email, причина)
- **LogEntry.cs** - Запись лога (пользователь, действие, цель, детали)

### Services

- **UserService.cs** - Регистрация, логин, управление пользователями
- **ChatService.cs** - Группы, участники, сообщения
- **ChannelService.cs** - Каналы, подписка, сообщения
- **PrivateChatService.cs** - Личные чаты, сообщения
- **AdminService.cs** - Бан/разбан, администрирование
- **LogService.cs** - Логирование действий
- **SettingsService.cs** - Сохранение/загрузка настроек
- **ContactService.cs** - Контакты, переименование
- **FavoriteService.cs** - Избранные сообщения
- **FileDialogService.cs** - Диалоги выбора файлов
- **ThemeService.cs** - Применение тем

### ViewModels

- **MainWindowViewModel.cs** - Главное окно, навигация
- **LoginViewModel.cs** - Страница входа
- **RegisterViewModel.cs** - Страница регистрации
- **ChatsViewModel.cs** - Список чатов, сообщения
- **ChatSettingsViewModel.cs** - Настройки чата
- **ChannelMembersViewModel.cs** - Участники канала (поиск)
- **CreateGroupViewModel.cs** - Создание группы
- **CreateChannelViewModel.cs** - Создание канала
- **CreateChatViewModel.cs** - Создание чата
- **SettingsViewModel.cs** - Настройки приложения
- **TerminalViewModel.cs** - Терминал администратора
- **UserProfileViewModel.cs** - Профиль пользователя

### Views

- **MainWindow.axaml/.cs** - Главное окно
- **LoginView.axaml/.cs** - Страница входа
- **RegisterView.axaml/.cs** - Страница регистрации
- **ChatsView.axaml/.cs** - Список чатов
- **ChatSettingsView.axaml/.cs** - Настройки чата
- **ChannelMembersView.axaml/.cs** - Участники канала
- **CreateGroupView.axaml/.cs** - Создание группы
- **CreateChannelView.axaml/.cs** - Создание канала
- **CreateChatView.axaml/.cs** - Создание чата
- **SettingsView.axaml/.cs** - Настройки приложения
- **TerminalView.axaml/.cs** - Терминал администратора
- **UserProfileView.axaml/.cs** - Профиль пользователя

### Converters (Конвертеры)

- **BoolConverters.cs** - Конвертеры для XAML-привязок

### Базы данных (JSON)

- *Примечание: Проект их может создавать сам*
- **users_data.json** - Личные данные пользователя (Пароль почта никнейм)
- **chats_data.json** - Все группы пользователей (Сообщения названия ID Аватарка Пользователи)
- **channels_data.json** - Все каналы пользователей (Сообщение названия Id Аватарка Пользователи)
- **userstheirchats_base.json** - Личные чаты между двумя пользователями (Участники Сообщения)
- **contacts.json** - Все контакты пользователей (CustomName Пользователь ВладелецКонтакта)
- **Favorites.json** - Хранит сообщения в избранное (Пользователь Сообщения)
- **userseitings_data.json** - Личные настройки каждого пользователя
- **userschatsthemes.json** - Темы и Обои пользователей / *Устарело*
- **logstelegrom.json** - Логи пользователей
- **banned_users.json** - Список забаненых участников Telegrom
- **admins.json** - Список администраторов Telegrom

# Как скачать Telegrom?

1. Скачиваем проект кнопкой Code => Download zip
2. После извлекаем все файлы через ваш .zip помощник на рабочий стол или в любую вашу папку на компьютере
3. После извлекаем файл в dotnet-sdk-10.7z через ваш .zip помощник в эту же папку и скачиваем содержимое
(dotnet 10 если его у вас нет или же старой версии)
4. После запускаем ярлык "ЗАПУСТИТЬ ПРИЛОЖЕНИЕ" и пользуемся.
Более точный гайд вы можете найти в этом же архиве, .txt файл.
