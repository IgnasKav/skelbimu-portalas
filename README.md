<h1>Required tools</h1>
<ul>
  <li>
    .NET latest version, you can download it from here: https://dotnet.microsoft.com/download
  </li>
  <li>
    IDE which supports .NET, I recommend using Visual Studio or Rider
  </li>
</ul>

<h1>Installation</h1>
<ul>
  <li>
    clone repository
  </li>
</ul>
<h3>You are done! You can start the project in one of the following ways</h3>
<ul>
  <li>
    open cloned repository using terminal and use "dotnet run" command
  </li>
  <li>
    open project with your selected IDE, select API project and press start button
  </li>
</ul>

<h1>Migrations</h1>
<h3>To add a database migration, execute following command in "Reactivities" directory</h3>
<ul>
  <li>
  dotnet ef migrations add "MigrationName" -p Persistence/ -s API/
  </li>
</ul>