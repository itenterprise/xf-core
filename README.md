#xf-core  IT-Enterprise
<h3>Использование:</h3>
<ol>
<li>Подключить проекты в решение (solution).</li>
<li>Добавить ссылки на проекты xf-core для своих проектов.</li>
<li>Пронаследовать класс App от Common.Core.ApplicationBase.</li>
</ol>

При проблемах с ссылками в Common проектах необходимо:
<ul>
<li>Удалить дескриптор Target в каждом из проектных файлов (*.csproj).</li>
<li>Переустановить NuGet пакеты.</li>
</ul>
