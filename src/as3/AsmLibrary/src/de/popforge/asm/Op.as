package de.popforge.asm
{
	//TODO add documentation for operations
	
	/**
	 * The Op class stores prototypes that correspond to IL instructions for the AVM2.
	 * 
	 * @author Joa Ebert
	 */	
	public final class Op
	{
		public static function add():void{}
		public static function addInt():void{}
		public static function asType(multiname:String):void{}
		public static function asTypeLate():void{}
		public static function and():void{}
		public static function not():void{}
		public static function or():void{}
		public static function xor():void{}
		public static function call(numArguments:int):void{}
		//unsupported//public static function callMethod(a0: *, a1: *):void{}
		public static function callProperty(multiname:String,numArguments:int):void{}
		public static function callPropertyLex(multiname:String,numArguments:int):void{}
		public static function callPropertyVoid(multiname:String,numArguments:int):void{}
		//unsupported//public static function callStatic(a0: *, a1: *):void{}
		public static function callSuper(multiname:String,numArguments:int):void{}
		public static function callSuperVoid(multiname:String,numArguments:int):void{}
		public static function checkFilter():void{}
		public static function coerce(multiname:String):void{}
		public static function coerceAny():void{}
		public static function coerceString():void{}
		public static function construct(numArguments:int):void{}
		public static function constructProperty(multiname:String,numArguments:int):void{}
		public static function constructSuper(numArguments:int):void{}
		public static function toBool():void{}
		public static function toInt():void{}
		public static function toDouble():void{}
		public static function toObject():void{}
		public static function toUInt():void{}
		public static function toString():void{}
		//unsupported//public static function debug(a0: *, a1: *, a2: *, a3: *):void{}
		//unsupported//public static function debugFile(a0: *):void{}
		//unsupported//public static function debugLine(a0: *):void{}
		public static function decLocalInt(register:int):void{}
		public static function decrement():void{}
		public static function decrementInt():void{}
		public static function deleteProperty(multiname:String):void{}
		public static function divide():void{}
		public static function duplicate():void{}
		public static function defaultXMLNamespace(namespace_:String):void{}
		public static function defaultXMLNamespaceLate():void{}
		public static function equals():void{}
		public static function escapeXMLAttribute():void{}
		public static function escapeXMLElement():void{}
		public static function findProperty(multiname:String):void{}
		public static function findPropertyStrict(multiname:String):void{}
		public static function getDescendants(multiname:String):void{}
		public static function getGlobalScope():void{}
		public static function getGlobalSlot(index:int):void{}
		public static function getLex(multiname:String):void{}
		public static function getLocal(register:*):void{}
		public static function getLocal0():void{}
		public static function getLocal1():void{}
		public static function getLocal2():void{}
		public static function getLocal3():void{}
		public static function getProperty(multiname:String):void{}
		public static function getScopeObject(index:int):void{}
		public static function getSlot(index:int):void{}
		public static function getSuper(multiname:String):void{}
		public static function greaterEquals():void{}
		public static function greaterThan():void{}
		public static function hasNext():void{}
		public static function hasNext2(objectRegister:int,indexRegister:int):void{}
		public static function ifEqual(label:String):void{}
		public static function ifFalse(label:String):void{}
		public static function ifGreaterEqual(label:String):void{}
		public static function ifGreaterThan(label:String):void{}
		public static function ifLessEqual(label:String):void{}
		public static function ifLessThan(label:String):void{}
		public static function ifNotGreaterEqual(label:String):void{}
		public static function ifNotGreaterThan(label:String):void{}
		public static function ifNotLessEqual(label:String):void{}
		public static function ifNotLessThan(label:String):void{}
		public static function ifNotEqual(label:String):void{}
		public static function ifStrictEqual(label:String):void{}
		public static function ifStrictNotEqual(label:String):void{}
		public static function ifTrue(label:String):void{}
		public static function in_():void{}
		public static function incLocal(register:int):void{}
		public static function incLocalInt(register:int):void{}
		public static function increment():void{}
		public static function incrementInt():void{}
		public static function initProperty(multiname:String):void{}
		public static function instanceOf():void{}
		public static function isType(multiname:String):void{}
		public static function isTypeLate():void{}
		public static function jump(label:String):void{}
		public static function kill(register:int):void{}
		public static function label():void{}
		public static function lessEquals():void{}
		public static function lessThan():void{}
		//unsupported//public static function lookUpSwitch(defaultLabel:String, ... caseLabels):void{}
		public static function shiftLeft():void{}
		public static function modulo():void{}
		public static function multiply():void{}
		public static function multiplyInt():void{}
		public static function negate():void{}
		public static function negateInt():void{}
		public static function newActivation():void{}
		public static function newArray(numElements:int):void{}
		//unsupported//public static function newCatch(a0: *):void{}
		//unsupported//public static function newClass(a0: *):void{}
		//unsupported//public static function newFunction(a0: *):void{}
		public static function newObject(numElements:int):void{}
		public static function nextName():void{}
		public static function nextValue():void{}
		public static function nop():void{}
		public static function notBool():void{}
		public static function pop():void{}
		public static function popScope():void{}
		public static function pushByte(byte:int):void{}
		public static function pushDouble(double:Number):void{}
		public static function pushFalse():void{}
		public static function pushInt(integer:int):void{}
		public static function pushNamespace(namespace_:String):void{}
		public static function pushNaN():void{}
		public static function pushNull():void{}
		public static function pushScope():void{}
		public static function pushShort(short:int):void{}
		public static function pushString(string:String):void{}
		public static function pushTrue():void{}
		public static function pushUInt(uinteger:uint):void{}
		public static function pushUndefined():void{}
		public static function pushWith():void{}
		public static function returnValue():void{}
		public static function returnVoid():void{}
		public static function shiftRight():void{}
		public static function setLocal(register:*):void{}
		public static function setLocal0():void{}
		public static function setLocal1():void{}
		public static function setLocal2():void{}
		public static function setLocal3():void{}
		public static function setGlobalSlot(index:int):void{}
		public static function setProperty(multiname:String):void{}
		public static function setSlot(index:int):void{}
		public static function setSuper(multiname:String):void{}
		public static function strictEquals():void{}
		public static function subtract():void{}
		public static function subtractInt():void{}
		public static function swap():void{}
		public static function throw_():void{}
		public static function typeOf():void{}
		public static function shiftRightUInt():void{}
	}
}