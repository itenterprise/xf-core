<h1>xf-core  IT-Enterprise</h1>
<h2>Установка ядра XamarinForms.IT-Enteprise</h2>
Standard: установить nuget-пакет IT-Enterprise.Xamarin.CoreStandard.<br>
Android: установить nuget-пакеты IT-Enterprise.Xamarin.CoreStandard и IT-Enterprise.Xamarin.CoreDroid.<br>
iOS: установить nuget-пакеты IT-Enterprise.Xamarin.CoreStandard и IT-Enterprise.Xamarin.CoreiOS.<br>
UWP: установить nuget-пакеты IT-Enterprise.Xamarin.CoreStandard и IT-Enterprise.Xamarin.CoreUWP.<br>

Для iOS и Android проектов добавить ресурсы из папки Resources.

Далее необходимо в проекте с общим кодом наследовать класс приложения от  ApplicationBase из библиотеки Common.CoreStandard. В классе приложения необходимо реализовать метод GotoMainPage и свойство AppId. Метод GotoMainPage переход к главной (стартовой) странице приложения. Например, если главной страницей приложения является объект класса ItMainPage, то реализация метода будет выглядеть следующим образом: 

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

<h2>Настройка использования ядра Android XamarinForms.IT-Enterprise</h2>
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

<h2>Настройка использования ядра iOS XamarinForms.IT-Enterprise</h2>
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
