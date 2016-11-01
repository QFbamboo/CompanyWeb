function ajax(url, onsuccess)
{
    var xmlhttp = window.XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject('Microsoft.XMLHTTP'); //创建XMLHTTP对象，考虑兼容性
    xmlhttp.open("POST", url, true); 
    xmlhttp.onreadystatechange = function ()
    {
        if (xmlhttp.readyState == 4) //readyState == 4 表示服务器返回完成数据了。之前可能会经历2（请求已发送，正在处理中）、3（响应中已有部分数据可用了，但是服务器还没有完成响应的生成）
        {
            if (xmlhttp.status == 200) //如果状态码为200则是成功
            {
                onsuccess(xmlhttp.responseText);
            }
            else
            {
                alert("AJAX服务器返回错误！");
            }
        }
    }
    //不要以为if (xmlhttp.readyState == 4) {在send之前执行！！！！
    xmlhttp.send(); //这时才开始发送请求
}