# Entry points:

## Task
Two applications communicate with each other through a transport mechanism, implementing the calculation of Fibonacci numbers.

The logic of calculating one sequence is as follows:

>1. The first initializes the calculation.
>2. The first sends N(i) to the second.
>3. The second calculates N(i-1) + N(i) and sends it back.
>4. The logic repeats symmetrically.

This continues until the applications stop.

### Pin points
>1. The first application receives a parameter at the start, an integer indicating how many asynchronous calculations to begin.
>2. All calculations are done in parallel.
>3. Data transmission from 1 to 2 goes through a Rest WebApi. Data transmission from 2 to 1 is done through MessageBus.

# Author comments:

Initially, the decision was made to perform calculations exclusively on the host. However, as can be inferred from the conditions, this process should alternate, i.e., both on the host and on the client (chunk by chunk), which was done.

There is no requirement on how to output the results, so the result is printed in log.Info.

The client takes a master position in relation to the host, so if the calculation can be completed BEFORE calling the host's endpoints, the host will NOT be additionally notified of the calculation results.

A summary overview is intentionally omitted because the requirements do not specify it, and the proper formatting of the final product will take some time (it's not laziness but an attempt to be efficient).

Interaction with the MessageBus can be (and probably should be, based on best practices) extracted into a separate service, which would help simplify migration from one service to another. This was not done because, within the scope of solving the current task, it was considered overkill and could significantly complicate analysis.
