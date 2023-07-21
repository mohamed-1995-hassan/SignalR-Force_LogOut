console.log('hello lol')
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var Id = ''
connection.start().then(() => connection.invoke("GetConnectionId")).then((connectionId) => {
    Id = connectionId;
    console.log(Id);
}).catch(function (err) {
    return console.error(err.toString());
});



connection.on("NotifyLogOut", () => {

    fetch("/Logout", { method: "POST" })
})

