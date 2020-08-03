function signIn() {
    let username = document.getElementById("username").value;
    $.get("gib public key",
    function(data) {
        alert('page content: ' + data);
     });
    $.post("log in request");
}