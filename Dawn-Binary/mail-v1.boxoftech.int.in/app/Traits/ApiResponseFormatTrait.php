<?php

namespace App\Traits;


trait ApiResponseFormatTrait {

 public static function jsonResponse($response, $code, $message, $successValue, $failureValue){
    $data =  [
        'response'   => $response,
        "failure" => [
            "code"=> ($response ? -1 : $code),
            "message"=> ($response ? "" : $message),
            "description"=> $failureValue,
            "trace"=> "",
        ],
        "success" => [
            "code"=>($response ? $code : -1),
            "message"=> ($response ? $message : ""),
            "description"=> '',
            "result"=> $successValue,
        ],
    ];
    return response()->json($data);
    }
}
