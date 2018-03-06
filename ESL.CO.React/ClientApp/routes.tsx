import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { KanbanBoard } from './components/KanbanBoard';
import { BoardReader } from './components/BoardReader';
import { StatisticsList } from './components/StatisticsList';

export const routes = <Layout>
    <Route exact path='/' component={BoardReader} />
    <Route path='/boardlist' component={BoardList} />
    <Route path='/kanbanboard' component={KanbanBoard} />
    <Route path='/statistics' component={StatisticsList} />
</Layout>;