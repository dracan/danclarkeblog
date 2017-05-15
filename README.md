## Dan Clarke's Blog

This repository contains the source code that runs my blog, which can be found at https://www.danclarke.com.

My post talking about it can [be found here](https://www.danclarke.com/blog-rewrite).

It's worthing pointing out, that whilst you're free to clone this repository and do whatever you like with it - I am not planning on supporting backwards compatibility. This is ***not*** a *supported* blogging platform - it's just the source code that drives my blog.

That said, I've exposed all the settings - eg. Dropbox access token, connection strings, hostname, etc - so that they can be set via either environment variables or application settings - so that I don't have any secrets stored in Git, ***but also*** so that other people can use this if they wish, and reference their own Dropbox and Azure accounts.

I'll also document in this repository's wiki how to set this up, how the Dropbox directory structure should be laid out, how to setup [VSTS](https://www.visualstudio.com/team-services/) and Azure to build/deploy/host this solution.

Happy Blogging! :)
