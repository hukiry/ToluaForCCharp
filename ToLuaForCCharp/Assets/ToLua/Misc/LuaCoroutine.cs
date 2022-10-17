
using LuaInterface;
using System;
using System.Collections;

public static class LuaCoroutine
{
	private static string strCo =
        @"
        local _WaitForSeconds, _WaitForFixedUpdate, _WaitForEndOfFrame, _Yield, _StopCoroutine = WaitForSeconds, WaitForFixedUpdate, WaitForEndOfFrame, Yield, StopCoroutine        
        local error = error
        local debug = debug
        local coroutine = coroutine
        local comap = {}
        setmetatable(comap, {__mode = 'k'})

        function _resume(co)
            if comap[co] then
                comap[co] = nil
                local flag, msg = coroutine.resume(co)
                    
                if not flag then
                    msg = debug.traceback(co, msg)
                    error(msg)
                end
            end        
        end

        function WaitForSeconds(t)
            local co = coroutine.running()
            local resume = function()                    
                _resume(co)                     
            end
            
            comap[co] = _WaitForSeconds(t, resume)
            return coroutine.yield()
        end

        function WaitForFixedUpdate()
            local co = coroutine.running()
            local resume = function()          
                _resume(co)     
            end
        
            comap[co] = _WaitForFixedUpdate(resume)
            return coroutine.yield()
        end

        function WaitForEndOfFrame()
            local co = coroutine.running()
            local resume = function()        
                _resume(co)     
            end
        
            comap[co] = _WaitForEndOfFrame(resume)
            return coroutine.yield()
        end

        function Yield(o)
            local co = coroutine.running()
            local resume = function()        
                _resume(co)     
            end
        
            comap[co] = _Yield(o, resume)
            return coroutine.yield()
        end

        function StartCoroutine(func)
            local co = coroutine.create(func)                       
            local flag, msg = coroutine.resume(co)

            if not flag then
                msg = debug.traceback(co, msg)
                error(msg)
            end

            return co
        end

        function StopCoroutine(co)
            local _co = comap[co]

            if _co == nil then
                return
            end

            comap[co] = nil
            _StopCoroutine(_co)
        end
        ";

}

