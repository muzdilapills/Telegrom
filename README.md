# Telegrom by Muzdilapills

## Описание Telegrom
### Что такое Telegrom?
Telegrom - Это мессенджер с открытым исходным кодом, с оптимизацией и многим другим.



#№ Структура проекта Telegrom

TelegromV4/
│
├── Models/
│   ├── User.cs
│   ├── Chat.cs
│   ├── Channel.cs
│   ├── Message.cs
│   ├── PrivateChat.cs
│   ├── Contact.cs
│   ├── FavoriteMessage.cs
│   ├── UserSettings.cs
│   ├── BannedUser.cs
│   └── LogEntry.cs
│
├── Services/
│   ├── UserService.cs
│   ├── ChatService.cs
│   ├── ChannelService.cs
│   ├── PrivateChatService.cs
│   ├── AdminService.cs
│   ├── LogService.cs
│   ├── SettingsService.cs
│   ├── ContactService.cs
│   ├── FavoriteService.cs
│   ├── FileDialogService.cs
│   └── ThemeService.cs
│
├── ViewModels/
│   ├── MainWindowViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ChatsViewModel.cs
│   ├── ChatSettingsViewModel.cs
│   ├── ChannelMembersViewModel.cs
│   ├── CreateGroupViewModel.cs
│   ├── CreateChannelViewModel.cs
│   ├── CreateChatViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── TerminalViewModel.cs
│   └── UserProfileViewModel.cs
│
├── Views/
│   ├── MainWindow.axaml
│   ├── MainWindow.axaml.cs
│   ├── LoginView.axaml
│   ├── LoginView.axaml.cs
│   ├── RegisterView.axaml
│   ├── RegisterView.axaml.cs
│   ├── ChatsView.axaml
│   ├── ChatsView.axaml.cs
│   ├── ChatSettingsView.axaml
│   ├── ChatSettingsView.axaml.cs
│   ├── ChannelMembersView.axaml
│   ├── ChannelMembersView.axaml.cs
│   ├── CreateGroupView.axaml
│   ├── CreateGroupView.axaml.cs
│   ├── CreateChannelView.axaml
│   ├── CreateChannelView.axaml.cs
│   ├── CreateChatView.axaml
│   ├── CreateChatView.axaml.cs
│   ├── SettingsView.axaml
│   ├── SettingsView.axaml.cs
│   ├── TerminalView.axaml
│   ├── TerminalView.axaml.cs
│   ├── UserProfileView.axaml
│   └── UserProfileView.axaml.cs
│
├── Converters/
│   └── BoolConverters.cs
│
├── App.axaml
├── App.axaml.cs
├── Program.cs
├── TelegromV4.csproj
├── app.manifest
│
├── JSON файлы / Базы данных
│   ├── users_data.json
│   ├── chats_data.json
│   ├── channels_data.json
│   ├── userstheirchats_base.json
│   ├── contacts.json
│   ├── Favorites.json
│   ├── userseitings_data.json
│   ├── userschatsthemes.json
│   ├── logstelegrom.json
│   ├── banned_users.json
│   └── admins.json
│
├── bin/
└── obj/

### Ничего не понял, за что файлы отвечают?

### Models (Модели данных)
User.cs - пользователь (ник, email, пароль, аватар)
Chat.cs - групповой чат (название, создатель, участники, сообщения)
Channel.cs - канал (название, создатель, подписчики, сообщения)
Message.cs - сообщение (отправитель, текст, время, вложение)
PrivateChat.cs - личный чат (два пользователя, сообщения)
Contact.cs - контакт (владелец, контакт, кастомное имя)
FavoriteMessage.cs - избранное сообщение (пользователь, текст, время)
UserSettings.cs - настройки пользователя (тема, приватность, аватар, обои)
BannedUser.cs - забаненный пользователь (ник, email, причина)
LogEntry.cs - запись лога (пользователь, действие, цель, детали)

### Services (Сервисы)
UserService.cs - регистрация, логин, получение пользователей
ChatService.cs - создание групп, добавление/удаление участников, сообщения
ChannelService.cs - создание каналов, подписка, сообщения
PrivateChatService.cs - личные чаты, сообщения
AdminService.cs - бан/разбан, выдача/снятие админа
LogService.cs - логирование действий
SettingsService.cs - сохранение/загрузка настроек
ContactService.cs - контакты, переименование
FavoriteService.cs - избранное
FileDialogService.cs - диалоги выбора файлов
ThemeService.cs - применение тем
ViewModels (ViewModel)
MainWindowViewModel.cs - главное окно, навигация
LoginViewModel.cs - логин
RegisterViewModel.cs - регистрация
ChatsViewModel.cs - список чатов, сообщения
ChatSettingsViewModel.cs - настройки чата
ChannelMembersViewModel.cs - список участников канала
CreateGroupViewModel.cs - создание группы
CreateChannelViewModel.cs - создание канала
CreateChatViewModel.cs - создание чата
SettingsViewModel.cs - настройки приложения
TerminalViewModel.cs - терминал администратора
UserProfileViewModel.cs - профиль пользователя

### Views (Представления)
MainWindow.axaml/.cs - главное окно
LoginView.axaml/.cs - окно входа
RegisterView.axaml/.cs - окно регистрации
ChatsView.axaml/.cs - список чатов
ChatSettingsView.axaml/.cs - настройки чата
ChannelMembersView.axaml/.cs - список участников канала
CreateGroupView.axaml/.cs - создание группы
CreateChannelView.axaml/.cs - создание канала
CreateChatView.axaml/.cs - создание чата
SettingsView.axaml/.cs - настройки приложения
TerminalView.axaml/.cs - терминал администратора
UserProfileView.axaml/.cs - профиль пользователя

### JSON файлы / Базы данных
users_data.json - данные пользователей
chats_data.json - данные групповых чатов
channels_data.json - данные каналов
userstheirchats_base.json - личные сообщения
contacts.json - контакты пользователей
Favorites.json - избранные сообщения
userseitings_data.json - настройки приватности
userschatsthemes.json - темы и обои
logstelegrom.json - логи действий
banned_users.json - забаненные пользователи
admins.json - администраторы

# Как скачать Telegrom?

1. Скачиваем проект кнопкой Code => Download zip
2. После извлекаем все файлы через ваш .zip помощник на рабочий стол или в любую вашу папку на компьютере
3. После извлекаем файл в dotnet-sdk-10.7z через ваш .zip помощник в эту же папку и скачиваем содержимое
(dotnet 10 если его у вас нет или же старой версии)
4. После запускаем ярлык "ЗАПУСТИТЬ ПРИЛОЖЕНИЕ" и пользуемся.
Более точный гайд вы можете найти в этом же архиве, .txt файл.
