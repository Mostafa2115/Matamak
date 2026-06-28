$conn = New-Object System.Data.SqlClient.SqlConnection("Server=localhost;Database=ResturantSys;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True")
$conn.Open()

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT Id, orderNumber, Discriminator FROM Orders"
$reader = $cmd.ExecuteReader()

while ($reader.Read()) {
    $id = $reader["Id"]
    $ordNum = $reader["orderNumber"]
    $type = $reader["Discriminator"]
    Write-Output "ID: $id | orderNumber: $ordNum | Type: $type"
}
$reader.Close()
$conn.Close()
