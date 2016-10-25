#xf-core  IT-Enterprise
##Настройка приложения для корректного использования ядра XamarinForms.IT-Enteprise:
После создания приложения Xamarin.Forms Portable в Visual Studio необходимо добавить зависимости на библиотеки ядра и NuGet-пакеты которые используются в ядре. 
Библиотеки ядра XamarinForms.IT-Enterprise:
-	Common.Core – библиотека общего кода для приложений Xamarin Forms;
-	Common.EUSign – библиотека для работы с ЕЦП;
-	Plugin.FilePicker – плагин для визуального выбора файлов в мобильном приложении.

Для добавления зависимостей на библиотеки необходимо скопировать библиотеки ядра из репозитория GitHub (папка «dll-s») и добавить зависимости на библиотеки ядра для каждого из проектов в соответствии мобильной платформой.
Для добавления NuGet-пакетов необходимо: 
-	В дереве решения нажать ПКМ на названии решения;
-	Выбрать пункт меню Manage NuGet Packages for Solution;
-	Установить все NuGet пакеты, которые приведены ниже, для всех проектов приложения.
NuGet пакеты которые необходимо установить:
-	Acr.UserDialogs – всплывающие окна и уведомления;
-	FrazzApps.Xamarin.Forms.AnalyticsConnector – работа с Google Analytics;
-	Microsoft.Bcl и Microsoft.Bcl.Build – поддержка типов, добавленных в более поздних версиях .NET;
-	Microsoft.Net.Http – поддержка работы по HTTP;
-	Newtonsoft.Json – работа с JSON;
-	PCLStorage – доступ к файловой системе мобильных устройств;
-	Plugin.Permissions – проверка и запросы на использование ресурсов мобильных устройств;
-	Plugin.Share – работа с функцией «Поделиться» и открытием браузера;
-	Xam.Plugin.Badge – отображение бейджей на иконке приложения;
-	Xam.Plugin.Connectivity – проверка наличия и информация о сетевом подключении;
-	Xam.Plugin.DeviceInfo – информация о мобильном устройстве;
-	Xam.Plugin.Media – создание фото/видео или выбор из галереи;
-	Xam.Plugin.Version – определение версии приложения;
-	Xam.Plugins.Messaging – звонки, смс, отправка e-mail;
-	Xam.Plugins.Notifier – локальные уведомления;
-	Xam.Plugins.Settings – кроссплатформенные настройки;
-	Xamarin.FFImageLoading, Xamarin.FFImageLoading.Forms и
Xamarin.FFImageLoading.Transformations – работа с картинками (округление углов, поворот и др.);
-	ZXing.Net.Mobile и ZXing.Net.Mobile.Forms – сканирование штрих- и QR-кодов.

Далее необходимо в проекте с общим кодом наследовать класс приложения от  ApplicationBase из библиотеки Common.Core. В классе приложения необходимо реализовать метод GotoMainPage и свойство AppId. Метод GotoMainPage переход к главной (стартовой) странице приложения. Например, если главной страницей приложения является объект класса ItMainPage, то реализация метода будет выглядеть следующим образом: 

```csharp
  public override void GotoMainPage()
 	{
		if (MainPage == null)
    {
      Device.BeginInvokeOnMainThread(() => MainPage = new ItMainPage());
      return;
    }
    MainPage.Navigation.PushModalAsync(new ItMainPage());
  }
```

Свойство AppId указывает на кодовое название приложения которое будет использоваться при проверке версии приложения и регистрации устройства на push-уведомления и др.
В этом классе доступны для переопределения и вызова следующие свойства и методы:
-	NotificationSenderId. Свойство, которое указывает идентификатор отправителя мгновенных сообщений (используется в Android проекте). По умолчанию равно null;
-	AppName. Свойство, которое указывает название приложения. По умолчанию равно AppId;
-	SystemModule. Свойство, которое указывает модуль системы IT-Enterprise к которому относиться приложение. По умолчанию равно пустой строке;
-	CurrentObject. Свойство, которое указывает объект системы IT-Enterprise к которому относиться приложение. По умолчанию равно пустой строке;
-	DatabaseStorage. Свойство, которое указывает уникальный идентификатор для сохранения данных в мобильной БД (если используется). По умолчанию равно комбинации идентификатора приложения и логина авторизированного в приложении пользователя;
-	CompanyName. Свойство, которое указывает название компании разработчика приложения. По умолчанию равно «IT-Enterprise»;
-	PushNotificationsEnabled. Свойство, которое определяет признак использования push-уведомлений. По умолчанию равно false;
-	PushNotificationsEnabled. Свойство, которое указывает адрес веб-сервисов, который будет использоваться по умолчанию. По умолчанию равно «https://demo-mobile.it.ua/ws/webservice.asmx»;
-	AllowToChangeUrl. Свойство, которое определяет признак разрешения на изменение адреса веб-сервисов. По умолчанию равно true;
-	MainAppColor. Свойство, которое определяет «главный» цвет приложения. По умолчанию равно Color.FromHex("#77D065");
-	Current. Свойство, которое указывает на текущий экземпляр приложения;
-	IsNetworkRechable. Свойство, которое определяет доступ к сетевому подключению;
-	AuthenicationPage. Свойство, которое указывает страницу аутентификации;
-	LoginPage. Свойство, которое указывает страницу ввода логина/пароля;
-	void Initialize(). Метод инициализации приложения. Будет вызван первым при после запуска приложения;
-	void SubscribePushNotifications(bool silentMode = false). Подписаться на прием push-уведомлений;
-	void UnsubscribePushNotifications(). Отписаться от push-уведомлений;
-	void StartFlow(). Метод будет вызываться после успешной инициализации приложения;
-	void StartRegistrationFlow(). Метод для вызова страницы-приветствия после инициализации приложения или после выполнения выхода из учетной записи пользователя;
-	void GotoAuthenticationPage(). Метод, который вызывается для отображения страницы аутентификации пользователя.

##Настройка использования ядра Android XamarinForms.IT-Enterprise
В проекте Android необходимо создать класс, описывающий Android-приложение. Этот класс должен наследовать класс MainApplicationBase из библиотеки Common.CoreDroid и для него должен быть задан атрибут [Application]. В этом классе есть возможность переопределить свойство NotificationSenderId и метод NotificationParameters GetNotificationParameters (NotificationPayload notificationPayload). Свойство NotificationSenderId указывает идентификатор отправителя push-уведомлений. В методе GetNotificationParameters задается процедура разбора данных из push-уведомления.
Также в проекте Android необходимо чтобы класс главной активности наследовал класс MainActivityBase из библиотеки Common.CoreDroid. В классе главной активности необходимо реализовать методы CreateApplication и OnCreate. 
Метод CreateApplication возвращает объект класса приложения из проекта общего кода. Например, если приложение описывается классом App, то реализация метода будет выглядеть следующим образом:

```csharp
  public override ApplicationBase CreateApplication()
  {
    return new App();
  }
```

Метод OnCreate вызывается при создании главной активности приложения Android. В нем необходимо вызвать реализацию этого же метода из родительского класса MainActivityBase. Пример:

```csharp
  protected override void OnCreate(Bundle bundle)
  {
    base.OnCreate(bundle);
    ...
  }
```

##Настройка использования ядра iOS XamarinForms.IT-Enterprise
В проекте iOS необходимо чтобы класс делегата приложения наследовал класс App из библиотеки Common.CoreiOS. В классе делегата приложения необходимо реализовать методы CreateApplication и FinishedLaunching, а также свойства, которые описывают цвета приложения:
-	ApplicationBase CreateApplication(). Метод, который возвращает объект класса приложения из проекта общего кода. Например, если приложение описывается классом App, то реализация метода будет выглядеть следующим образом:

```csharp
  public override ApplicationBase CreateApplication()
  {
    return new App();
  }
```

-	bool FinishedLaunching (UIApplication uiApplication, NSDictionary launchOptions). Метод, который вызывается после запуска приложения. В нем необходимо вызвать реализацию этого же метода из родительского класса делегата приложения. Например:

```csharp
  public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
  {
    var result = base.FinishedLaunching(uiApplication, launchOptions);
    ...
    return result;
  }
```

-	BarTintColor. Свойство, которое указывает цвет полосы заголовка приложения;
-	BarTitleColor. Свойство, которое указывает цвет текста заголовка приложения;
-	TintColor. Свойство, которое указывает цвет выделения в приложении.
