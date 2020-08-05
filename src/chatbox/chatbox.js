function signIn() {
    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;
    $.get("gib public key",
        function(data) {
            alert(data);
            $.post("login request", 
            RSA.encrypt("username: " + username + "password: " + password, data));
        });
}