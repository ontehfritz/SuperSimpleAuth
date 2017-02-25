# Super Simple Auth (SSA) Micro-Service Note: This is WIP
**Only use SSA with HTTPS**

Is a simple secure web authentication and authorization tool for any web based project.

### How is it secure and simple?
We provide an api for all the popular languages. The api libraries integrate with your web application and run server side.
Because it is server side and you enable https, there is only one point of insecurity, if and only if an attacker compromises
your server.

### What if someone compromises my server?
We add addtional features to only accept traffic coming from your server. You can add your server to the IP white list.
This way even if an attacker does compromise your domain key. The attacker will not be able to use it accept from an IP 
that is in your whitelist. 

### If an attacker compromises my domain key what is the worst case scenrio?
Because SSA is a multitier architecture each tier is protected. If an attacker does get your domian key the worst case is 
the attacker can create user ids. However mechanism have been put in place to minimize this. 

### Can I use this with client side applications? 
Currently I would not recommend this. If the software is smart client or strictly client side with js. It is not recommended. 

### Are you working on making SSA secure for client side?
Yes, we want this to be ubiquotis services. Keep an eye on the project for this feature.


