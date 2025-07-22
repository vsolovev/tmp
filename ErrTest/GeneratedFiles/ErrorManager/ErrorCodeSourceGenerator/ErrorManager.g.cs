using ErrTest;
public static class ErrorManager {
    public static class MyCompany {
        public static class MyService {
            public static class CartElement {
                public static class Request {
                    public static readonly ErrorCode Inaccessible = new ErrorCode("MyCompany.MyService.CartElement.Request.Inaccessible", "The requested card element is not accessible", 403);
                }
            }
            public static class CartContent {
                public static class Request {
                    public static readonly ErrorCode InternalError = new ErrorCode("MyCompany.MyService.CartContent.Request.InternalError", "Error requesting cart content", 500);
                }
            }
        }
    }
}
