--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
if jit then		
	if jit.opt then		
		jit.opt.start(3)				
	end		
	
	print("ver"..jit.version_num.." jit: ", jit.status())
	print(string.format("os: %s, arch: %s", jit.os, jit.arch))
end

if DebugServerIp then  
  require("mobdebug").start(DebugServerIp)
end

require "misc.functions"

json = require 'cjson'

list		= require "list"
utf8		= require "misc.utf8"

require "event"
require "typeof"
require "slot"
require "System.Timer"
require "System.coroutine"
require "System.ValueType"
require "System.Reflection.BindingFlags"

--require "misc.strict"