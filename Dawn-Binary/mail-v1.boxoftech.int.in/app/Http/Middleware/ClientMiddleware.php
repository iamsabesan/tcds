<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use App\Traits\ApiResponseFormatTrait;
use JWTAuth;
class ClientMiddleware
{
    use ApiResponseFormatTrait;
    /**
     * Handle an incoming request.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \Closure(\Illuminate\Http\Request): (\Illuminate\Http\Response|\Illuminate\Http\RedirectResponse)  $next
     * @return \Illuminate\Http\Response|\Illuminate\Http\RedirectResponse
     */
    public function handle(Request $request, Closure $next)
    {
        try {
        if($auth = $request->header('X-Authorization')) {
            	$request->headers->set('Authorization', $auth);
        }
        $user = JWTAuth::parseToken()->authenticate();

    } catch (\Exception $e) {
        if ($e instanceof \Tymon\JWTAuth\Exceptions\TokenInvalidException):
            return $this->jsonResponse(false, 401, 'Invalid Tokens', '', '');
            elseif ($e instanceof \Tymon\JWTAuth\Exceptions\TokenExpiredException):
            return $this->jsonResponse(false, 401, 'Token is expired', '', '');
        else:
            return $this->jsonResponse(false, 401, 'Authorization token not found', '', '');
        endif;
    }
    if($user):
        $request['userid'] = $user->userid;
        return $next($request);
    else:
        return $this->responseWithError(__('Invalid Token'),[],401);
    endif;
    }
}
