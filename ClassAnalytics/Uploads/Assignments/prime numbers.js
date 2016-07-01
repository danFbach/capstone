"use strict"
var primes = new findprimes();
primes.program(10000000);

function findprimes(){
	var primeList = [2];
	var primeCheck;
	this.program = function(limit){
		var dStart = new Date();
		var tStart = dStart.getTime();
		for(var i = 3;i < limit;i++){
			if(i % 2 !==0)
				{
				for(var x = 0;primeList[x]<=Math.sqrt(i);x++)
				{
					var result = i % primeList[x];
					if(result === 0){
						primeCheck = true;
						break;
					}else{
						primeCheck = false;
					}
				}
				if(!primeCheck){
				primeList.push(i);
				}
			}
		}
		var dEnd = new Date();
		var tEnd = dEnd.getTime();
		console.log("Time:", (tEnd-tStart)/1000);
		console.log('There are',primeList.length,'primes between 1 and',limit);
	}
}