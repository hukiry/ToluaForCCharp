
function testFuc( ... )
	-- body
    local x = nil
    if x>90 then
    print(x);
    end
end

--主入口函数。从这里开始lua逻辑
function Main()					
	print("Main start")

    local tab = {1,23,543,567,234} 
    for i, v in ipairs( tab ) do
    	print("--------------------------=", i, v )
    end  
    
    print( math.floor(23.9))
    local tst = json.encode({k=2})
    print(tst);
    xpcall(function() testFuc() end,function(msg) print(msg)  end)
end






