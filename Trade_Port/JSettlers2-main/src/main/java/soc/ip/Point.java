package soc.ip;

import java.util.Objects;

public class Point<A, B> {
    private A a;
    private B b;

    public Point(A a, B b) {
        this.a = a;
        this.b = b;
    }

    public A getA() { return a; }
    public B getB() { return b; }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Point)) return false;
        Point<?, ?> Point = (Point<?, ?>) o;
        return Objects.equals(a, Point.a) && Objects.equals(b, Point.b);
    }

    @Override
    public int hashCode() {
        return Objects.hash(a, b);
    }
}
