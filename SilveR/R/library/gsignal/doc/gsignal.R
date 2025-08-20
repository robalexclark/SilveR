## ----include = FALSE----------------------------------------------------------
knitr::opts_chunk$set(
  collapse = TRUE,
  comment = "#>"
)

## ----setup--------------------------------------------------------------------
library(gsignal)

## ----pulstran, fig.height=7, fig.width=7--------------------------------------

op <- par(mfrow = c(2, 2))

## periodic rectangular pulse
t <- seq(0, 60, 1/1e3)
d <- cbind(seq(0, 60, 2), sin(2 * pi * 0.05 * seq(0, 60, 2)))
y <- pulstran(t, d, 'rectpuls')
plot(t, y, type = "l", xlab = "", ylab = "",
     main = "Periodic rectangular pulse")

## assymetric sawtooth waveform
fs <- 1e3
t <- seq(0, 1, 1/fs)
d <- seq(0, 1, 1/3)
x <- tripuls(t, 0.2, -1)
y <- pulstran(t, d, x, fs)
plot(t, y, type = "l", xlab = "", ylab = "",
     main = "Asymmetric sawtooth ")

## Periodic Gaussian waveform
fs <- 1e7
tc <- 0.00025
t <- seq(-tc, tc, 1/fs)
x <- gauspuls(t, 10e3, 0.5)
ts <- seq(0, 0.025, 1/50e3)
d <- cbind(seq(0, 0.025, 1/1e3), sin(2 * pi * 0.1 * (0:25)))
y <- pulstran(ts, d, x, fs)
plot(ts, y, type = "l", xlab = "", ylab = "",
     main = "Gaussian pulse")

## Custom pulse trains
fnx <- function(x, fn) sin(2 * pi * fn * x) * exp(-fn * abs(x))
ffs <- 1000
tp <- seq(0, 1, 1 / ffs)
pp <- fnx(tp, 30)
fs <- 2e3
t <- seq(0, 1.2, 1 / fs)
d <- seq(0, 1, 1/3)
dd <- cbind(d, 4^-d)
z <- pulstran(t, dd, pp, ffs)
plot(t, z, type = "l", xlab = "", ylab = "",
     main = "Custom pulse")

par(op)


## ----findpeaks, fig.height=4, fig.width=7-------------------------------------

t <- 2 * pi * seq(0, 1,length = 1024)
y <- sin(3.14 * t) + 0.5 * cos(6.09 * t) +
     0.1 * sin(10.11 * t + 1 / 6) + 0.1 * sin(15.3 * t + 1 / 3)
data1 <- abs(y) # Positive values
peaks1 <- findpeaks(data1)
data2 <- y # Double-sided
peaks2 <- findpeaks(data2, DoubleSided = TRUE)
peaks3 <- findpeaks (data2, DoubleSided = TRUE, MinPeakHeight = 0.5)

op <- par(mfrow=c(1,2))
plot(t, data1, type="l", xlab="", ylab="")
points(t[peaks1$loc], peaks1$pks, col = "red", pch = 1)
plot(t, data2, type = "l", xlab = "", ylab = "")
points(t[peaks2$loc], peaks2$pks, col = "red", pch = 1)
points(t[peaks3$loc], peaks3$pks, col = "red", pch = 4)
par (op)
title("Finding the peaks of smooth data is not a big deal")

t <- 2 * pi * seq(0, 1, length = 1024)
y <- sin(3.14 * t) + 0.5 * cos(6.09 * t) + 0.1 *
     sin(10.11 * t + 1 / 6) + 0.1 * sin(15.3 * t + 1 / 3)
data <- abs(y + 0.1*rnorm(length(y),1))   # Positive values + noise
peaks1 <- findpeaks(data, MinPeakHeight=1)
dt <- t[2]-t[1]
peaks2 <- findpeaks(data, MinPeakHeight=1, MinPeakDistance=round(0.5/dt))
op <- par(mfrow=c(1,2))
plot(t, data, type="l", xlab="", ylab="")
points (t[peaks1$loc],peaks1$pks,col="red", pch=1)
plot(t, data, type="l", xlab="", ylab="")
points (t[peaks2$loc],peaks2$pks,col="red", pch=1)
par (op)
title(paste("Noisy data may need tuning of the parameters.\n",
            "In the 2nd example, MinPeakDistance is used\n",
            "as a smoother of the peaks"))


## ----fir, fig.height=4, fig.width=7-------------------------------------------

## FIR filter design by windowing

# low-pass filter 10 Hz
fs = 256
h <- fir1(40, 10/ (fs / 2), "low")
freqz(h, fs = fs)
# observe the effect of filter length
h <- fir1(80, 10/ (fs / 2), "low")
freqz(h, fs = fs)

# fir2 allows specifying arbitrary frequency responses
f <- c(0, 0.3, 0.3, 0.6, 0.6, 1)
m <- c(0, 0, 1, 1/2, 0, 0)
fh <- freqz(fir2(100, f, m))
op <- par(mfrow = c(1, 2))
plot(f, m, type = "b", ylab = "magnitude", xlab = "Frequency")
lines(fh$w / pi, abs(fh$h), col = "blue")
legend("topright", legend = c("specified", "actual"), lty = 1,
       pch = c(1, NA), col = c("black", "blue"))
# plot in dB:
plot(f, 20*log10(m+1e-5), type = "b", ylab = "dB", xlab = "Frequency")
lines(fh$w / pi, 20*log10(abs(fh$h)), col = "blue")
par(op)
title("specify arbitrary frequency responses with fir2")

## 50 Hz notch filter with remez
fs <- 200
nyquist <- fs / 2
f <- c(0, 48.5 / nyquist, 49.5 / nyquist, 50.5 / nyquist, 51.5 / nyquist, 1)
a <- c(1, 1, 0, 0, 1, 1)
h <- remez(200, f, a)
freqz(h, fs = fs)


## ----FIR_delay, fig.height=7, fig.width=7-------------------------------------

op <- par(mfrow = c(2, 1))
# design the filter
fs = 256
h <- fir1(40, 30/ (fs / 2), "low")

# group delay is constant at N/2
gd <- grpdelay(h)
plot(gd, ylim = c(0, 40),
     main = paste("(a) Group delay for FIR filters is constant\n",
                  "(here 40 / 2 = 20)"))

# filter electrocardiogram data with added noise
data(signals, package = "gsignal")
npts <- nrow(signals)
ecg <- signals$ecg + 1000 * runif(npts)
time <- seq(0, 10, length.out = npts)
plot(time, ecg, type = "l", main = "(b) Example ECG signal",
     xlab = "Time", ylab = "", xlim = c(0,2))
title(ylab = expression(paste("Amplitude (", mu, "V)")), line = 2)
f1 <- gsignal::filter(h, ecg)
lines(time, f1, col = "red", lwd = 2)
delay <- mean(gd$gd)
f2 <- c(f1[(delay + 1):npts], rep(NA, delay))
lines(time, f2, col = "blue", lwd = 2)
legend("topright", legend = c("Original", "Filtered", "Corrected"),
       lty = 1, lwd = c(1, 2, 2), col = c("black", "red", "blue"))

par(op)


## ----iir, fig.height=7, fig.width=7-------------------------------------------

op <- par(mfrow = c(3,1))

# compare Butterworth and Chebyshev filters.
bfr <- freqz(butter(5, 0.1))
cfr <- freqz(cheby1(5, .5, 0.1))
plot(bfr$w, 20 * log10(abs(bfr$h)), type = "l", ylim = c(-80, 0),
     xlab = "Frequency (rad)", ylab = c("dB"),
     main = "(a) Butterworth and Chebyshev")
lines(cfr$w, 20 * log10(abs(cfr$h)), col = "red")
legend("topright", legend = c("5th order Butterworth", "5th order Chebyshev"),
       lty = 1, col = c("black", "red"))

# compare Butterworth and elliptic filters.
efr <- freqz(ellip(5, 3, 40, 0.1))
plot(bfr$w, 20 * log10(abs(bfr$h)), type = "l", ylim = c(-80, 0),
    xlab = "Frequency (rad)", ylab = c("dB"),
    main = "(b) Butterworth and Elliptic")
lines(efr$w, 20 * log10(abs(efr$h)), col = "red")
legend ("topright", legend = c("5th order Butterworh", "5th order Elliptic"),
       lty = 1, col = c("black", "red"))

# compare type I and type II Chebyshev filters.
c1fr <- freqz(cheby1(5, .5, 0.1))
c2fr <- freqz(cheby2(5, 20, 0.1))
plot(c1fr$w, 20 * log10(abs(c1fr$h)), type = "l", ylim = c(-80, 0),
     xlab = "Frequency (rad)", ylab = c("dB"),
     main = "(c) Type I and II Chebyshev")
lines(c2fr$w, 20 * log10(abs(c2fr$h)), col = "red")
legend ("topright", legend = c("5th order Type I", "5th order Type II"),
       lty = 1, col = c("black", "red"))

par(op)

## ----zplane, fig.height=7, fig.width=7----------------------------------------

op <- par(no.readonly = TRUE)
n <- layout(matrix(c(1, 2, 3, 3), nrow = 2, byrow = TRUE))
stable <- butter(3, 0.2, "low", output = "Zpg")

# artificially adapt pole
instable <- stable
instable$p[2] <- instable$p[2] - 2

zplane(stable, main = "Stable")
zplane(instable, main = "Instable")

t <- seq(0, 1, len = 100)
x <- sin(2* pi * t * 2.3) + 0.5 * rnorm(length(t))
z1 <- filter(stable, x)
z2 <- filter(instable, x)
plot(t, x, type = "l", xlab = "", ylab = "")
lines(t, z1, col = "green", lwd = 2)
lines(t, z2, col = "red")
legend("bottomleft", legend = c("Original", "Stable", "Instable"),
       lty = 1, col = c("black", "green", "red"), ncol = 3)
par(op)


## ----IIR_delay, fig.height=7, fig.width=7-------------------------------------
op <- par(mfrow = c(2, 1))
ell <- ellip(5, 0.1, 60, 30/(fs/2), "low")
ellf <- freqz(ell, fs = fs)
argh <- Arg(ellf$h)
argh[which(is.na(argh))] <- 0
phase <- unwrap(argh)
plot(ellf$w, phase, type = "l", xlab = "Frequency (Hz)", ylab = "Phase",
     main = paste("30 Hz 5th order elliptical low-pass IIR filter\n",
                  "phase response is not linear"))
gd <- grpdelay(ell, fs = fs)
plot(gd, main = paste("group delay depends on frequency\n",
                      "mean:", round(mean(gd$gd), 1), "samples"))
par(op)


## ----IIR_filtfilt, fig.height=4, fig.width=7----------------------------------

f <- filter(ell, ecg)
ff <- filtfilt(ell, ecg)
plot(time, ecg, type = "l", xlab = "Time", ylab = "", xlim = c(0,2))
title(ylab = expression(paste("Amplitude (", mu, "V)")), line = 2)
lines(time, f, col = "red", lwd = 2)
lines(time, ff, col = "blue", lwd = 2)
legend("topright", legend = c("Original", "filter()", "filtfilt()"),
       lty = 1, lwd = c(1, 2, 2), col = c("black", "red", "blue"))

## ----conv1--------------------------------------------------------------------

u <- rep(1, 3)
v <- c(1, 1, 0, 0, 0, 1, 1)
conv(u, v, "full")
conv(u, v, "same")
conv(u, v, "valid")
conv(v, u, "valid")



## ----conv2, fig.height=7, fig.width=7-----------------------------------------

  short <- runif(20L)
  long <- runif(1000L)

  # convolve two long series
  ll <- microbenchmark::microbenchmark(conv(long, long),
                                       cconv(long, long),
                                       fftconv(long, long))
  plot1 <- ggplot2::autoplot(ll)

  # convolve a short and a long series
  sl <- microbenchmark::microbenchmark(conv(short, long),
                                       cconv(short, long),
                                       fftconv(short, long))
  plot2 <- ggplot2::autoplot(sl)
 
  gridExtra::grid.arrange(plot1, plot2, nrow = 2, ncol = 1)
  

## ----chunks-------------------------------------------------------------------

N <- 10000L
long <- runif(N)
part1 <- long[1:(N / 2)]
part2 <- long[((N / 2) + 1):N]

b <- c(2, 3)
a <- c(1, 0.2)

y <- filter(b, a, long)
y1 <- filter(b, a, part1, 'zf')
y2 <- filter(b, a, part2, y1$zf)
yy <- c(y1$y, y2$y)
all.equal(y, yy)



## ----zi, fig.height=5, fig.width=7--------------------------------------------

t <- seq(-1, 1, length.out =  201)
x <- (sin(2 * pi * 0.75 * t * (1 - t) + 2.1)
     + 0.1 * sin(2 * pi * 1.25 * t + 1)
     + 0.18 * cos(2 * pi * 3.85 * t))
h <- butter(3, 0.05)
zi <- filter_zi(h)
## alternatively, use:
## lab <- max(length(h$b), length(h$a)) - 1
## zi <- filtic(h, rep(1, lab), rep(1, lab))
z1 <- filter(h, x)
z2 <- filter(h, x, zi * x[1])
plot(t, x, type = "l", xlab ="", ylab = "")
lines(t, z1, col = "red")
lines(t, z2$y, col = "green")
legend("bottomright", legend = c("Original signal",
       "Filtered without initial conditions",
       "Filtered with initial conditions"),
      lty = 1, col = c("black", "red", "green"))



## ----fft_ar, fig.height=7, fig.width=7----------------------------------------

op <- par(mfrow = c(3, 1))
fs <- 200
nsecs <- 10
lx <- fs * nsecs
t <- seq(0, nsecs, length.out =  lx)
# signal of 5 Hz + 12 Hz + noise
x <- (sin(2 * pi * 5 * t)
     + sin(2 * pi * 12 * t)
     + runif(lx))
plot(t, x, type = "l", xlab = "Time (s)", ylab = "", main = "Original signal")
pw <- pwelch(x, window = lx, fs = fs, detrend = "none")
plot(pw, xlim = c(0, 20), main = "PSD estimate using FFT")

py <- pyulear(x, 30, fs = fs)
plot(py, xlim = c(0, 20), main = "PSD estimate using Yule-Walker")
par(op)


## ----welch, fig.height=7, fig.width=7-----------------------------------------

op <- par(mfrow = c(3, 1))
fs <- 200
nsecs <- 100
lx <- fs * nsecs
t <- seq(0, nsecs, length.out =  lx)
# sine and cosine of signal of 5 Hz noise
x1 <- cos(2 * pi * 5 * t) + runif(lx)
x2 <- sin(2 * pi * 5 * t) + runif(lx)
x <- cbind(x1, x2)
pw <- pwelch(x, fs = fs)
plot(pw, plot.type = "spectrum", yscale = "dB", xlim = c(0, 50),
    main = "A sine and a cosine of 5 Hz have the same PSD")
legend("topright", legend = c("Cosine", "Sine"), lty = 1:2, col = 1:2)
rect(3, -35, 7, -4, border = "red", lwd = 3)
plot(pw, plot.type = "phase", xlim = c(0, 50),
    main = expression(bold(paste("but differ ", pi/2, " radians in phase at 5 Hz"))))
rect(3, -pi, 7, pi, border = "red", lwd = 3)
plot(pw, plot.type = "coherence", xlim = c(0, 50),
    main = "leading to coherence ~ 1 at 5 Hz")
rect(3, 0, 7, 1, border = "red", lwd = 3)
par(op)


## ----specgram, fig.height=7, fig.width=7--------------------------------------

op <- par(mfrow = c(2, 1))

jet <- grDevices::colorRampPalette(c("#00007F", "blue", "#007FFF", "cyan", "#7FFF7F",
                                     "yellow", "#FF7F00", "red", "#7F0000"))
sp <- specgram(chirp(seq(0, 5, by = 1/8000), 200, 2, 500, "logarithmic"), fs = 8000)
plot(sp, col = jet(20))

c2w <- grDevices::colorRampPalette(colors = c("red", "white", "blue"))
plot(sp, col = c2w(50))

par(op)


