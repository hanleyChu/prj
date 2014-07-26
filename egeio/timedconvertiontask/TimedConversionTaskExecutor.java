class TimedConversionTaskExecutor{
    
        ConverterPool cpool;
        TimedConversionThreadPool internalExecutor;

        private final static class TimedConversionThreadPool extends ThreadPoolExecutor{
            SynchronizedHeap deadlineQueue;

            Thread monitor = new Thread(new Runnable(){
                run(){
                    for(;;){
                        try{
                            TimedConversionTask task = deadlineQueue.peek(); //blocking if none
                            sleep(task.getDeadline()-System.getCurrentTime());

                            if(!task.isDone()){
                                task.cancel(true);
                            }

                            deadlineQueue.remove(task);
                        }
                    }
                }
            });

            @Override
            protected <T> RunnableFuture<T> newTaskFor(Runnable runnable, T value) {
                return new TimedConversionTask<T>((TimedRunnable)runnable, null);
            }

            @Override
            protected void beforeExecute(Thread t, Runnable r) {
                // try to retrieve the converter instance
                Converter c = cpool.borrow()

                // attach the converter for this conversion
                TimedConversionTask task = (TimedConversionTask)r;
                task.timedConversion.c = c;

                // insert into deadlinePriorityQueue
                deadlineQueue.insert(task);

                // set the deadline when task is about to start
                task.setConverter(c)
                task.setDeadline(System.getCurrentTime()+timeLimit);

                // resort the deadlineQueue
                deadlineQueue.add(task)
                monitor.interrupt();
            }

            // for internally using, warpper of a TimeConversion, representing a task
            private final static class TimedConversionTask extends FutureTask{
                long deadline;
                TimedConversion timedConversion;

                public TimedConversionTask(TimedConversion tc){
                    super((Runnable)tc, null);
                    timedConversion = tc;
                }

                @Override
                public boolean cancel(boolean mayInterruptIfRunning) {
                    try{
                        timedConversion.c.close();
                    }finally{
                        return super.cancel(mayInterruptIfRunning);
                    }
                }
        }

        // conversion task specification
        public final static class TimedConversion implement Runnable{
            long timeLimit;

            private volatile Converter c;

            File dest;
            File src;

            TimedConversion(src,dest,timeLimit)

            run(){
                try{
                    // clear flag bit
                    Thread.interrupted()

                    // here jodconverter does not support timeout, so we use monitor thread to interrupt this one
                    // but actually use close socket instead of interupting because network io is not interruptable
                    c.convert(src,dest);
                    
                    ackSucess();
                }catch(SocketException){
                    if(Thread.interrupted()){ // timeout and cancelled
                        ackFailure();
                    }else{ // error occurs not timeout,clear flag bit
                        ackRetrial();
                    }
                }finally{
                    cpool.return(c);
                }
            }

            ackRetrial()
            ackFailure()
            ackSucess()
        }
    }

    submit(TimedConversion tc){
        internalExecutor.submit((Runnable)tc);
    }
}

