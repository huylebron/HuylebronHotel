﻿using Microsoft . AspNetCore . Http ;

namespace HotelProject . Domain . Model . Commons ;

public class ResponseResult
{
    protected ResponseResult ( int statusCode , string message ) {
        StatusCode = statusCode ;
        Message = message ;
    }

    public int StatusCode { get ; set ; }
    public string Message { get ; set ; }

    public static ResponseResult Success ( string message = null ) {
        return new ResponseResult ( StatusCodes . Status200OK , message ) ;
    }

    public static ResponseResult Fail ( string message = null ) {
        return new ResponseResult ( StatusCodes . Status400BadRequest , message ) ;
    }
}

public class ResponseResult < T > : ResponseResult
{
    protected ResponseResult ( int statusCode , string message , T data ) : base ( statusCode , message ) {
        Data = data ;
    }

    public T Data { get ; set ; }

    public static ResponseResult < T > Success ( T data , string message = null ) {
        return new ResponseResult < T > ( StatusCodes . Status200OK , message , data ) ;
    }
}