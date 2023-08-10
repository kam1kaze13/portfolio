export const errorHandler = 
{
    createResponse404(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 404,
            ReasonPhrase : "Not Found",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse304(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 304,
            ReasonPhrase : "Not Modified",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse416(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 416,
            ReasonPhrase : "Request Range Not Satisfiable",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse400(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 400,
            ReasonPhrase : "Bad Request",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse408(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 408,
            ReasonPhrase : "Request Timeout",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse500(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 500,
            ReasonPhrase : "Internal Server Error",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    },

    createResponse501(httpVersion)
    {
        return {
            HttpVersion: httpVersion,
            StatusCode : 501,
            ReasonPhrase : "Not Implemented",
            Date : new Date(),
            AcceptRanges : "bytes"
        };
    }
}