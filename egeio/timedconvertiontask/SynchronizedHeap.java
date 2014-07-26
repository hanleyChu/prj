import java.lang.*

public class SynchronizedHeap{

    private TimedConversionTask[] tasks = new TimedConversionTask[10] // skip 0-index element

    private int n = 0;

    private ReentrantLock lock;
    private Condition notEmpty = lock.newCondition();

    public void add(TimedConversionTask task){
        lock.lock();
        try{
            
            if(n==tasks.length){
                TimedConversionTask[] old = tasks;
                tasks = new TimedConversionTask[(int)(1.2*n)];
                Arrays.copy(old,tasks);
            }

            ++n;
            tasks[n] = task;
            swim(n);
            if(n>=1)
                notEmpty.signal();
        }finally{
            lock.unlock();
        }
    }

    public void remove(int k){
        lock.lock();
        try{
            TimedConversionTask t;
            t = tasks[k];tasks[k]=tasks[n];tasks[n]=t;
            --n;
            swim(k);
            sink(k);
        }finally{
            lock.unlock();
        }
    }

    public TimedConversionTask peek(){
        lock.lock();
        try{
            if(n==0)
                notEmpty.await();
                TimedConversionTask t;
                t = tasks[1];tasks[1]=tasks[n];
                tasks[n] = null; //help gc
                --n;
                return t;
        }finally{
            lock.unlock();
        }
    }

    private void swim(int k){
        int j;
        TimedConversionTask t;
        while(k>1){
            j = k/2;
            if(tasks[j].getDeadline() > tasks[k].getDeadline()){
                t = tasks[j];tasks[j]=tasks[k];tasks[k]=t;
            }else
                break;
            k = j;
        }
    }

    private void sink(int k){
        int j;
        TimedConversionTask t;
        while(2*k<=n){
            j = 2*k;
            if(j<n){
                if(tasks[j].getDeadline()>tasks[j+1].getDeadline())
                    ++j;
            }
            if(tasks[j].getDeadline()<tasks[k].getDeadline()){
                t = tasks[j];tasks[j] = tasks[k];tasks[k]=t;
            }
            else
                break;
            k = j;
        }
    }

    
}