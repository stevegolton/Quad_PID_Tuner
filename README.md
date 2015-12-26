# QuadMonitor
Simple C#/WinForms control panel for tuning PID gains.

This project is very much a work in progress. It currently uses a hacky message protocol I threw together to get something running with my DIY quad. The end goal is to get it working with the Mavlink [parameter protocol](http://qgroundcontrol.org/mavlink/parameter_protocol) which is the protocol I have since moved to for my quad. 

I created it because it's handy to have a nice slider style interface to use for tuning a quad's PID gains.

TODO
  - [ ] Use mavlink's [parameter protocol](http://qgroundcontrol.org/mavlink/parameter_protocol) to tune gains in order to provide compatibility with other mavlink supported quads.
  
Build Instructions
------------------
Built and tested with MS Visual Studio 2013 Express Edition.
