import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { KanbanBoard } from './components/KanbanBoard';
import { BoardReader } from './components/BoardReader';
import { StatisticsList } from './components/StatisticsList';
import { StatisticsBoard } from './components/StatisticsBoard';
import { Login } from './components/Login';
import { PresentationList } from './components/PresentationList';
import { NavMenu } from './components/NavMenu';

export const routes = <Layout>
    <Route path='/' component={NavMenu} />
    <Route exact path='/' component={BoardReader} />
    <Route path='/kanbanboard' component={KanbanBoard} />
    <Route path='/statistics' component={StatisticsList} />
    <Route path="/jiraconnectionstats/:id" component={StatisticsBoard} />
    <Route path='/admin/presentations' component={PresentationList} />
    <Route path='/admin/presentations/:id' component={BoardList} />
    <Route exact path='/admin' component={BoardList} />
    <Route path='/login' component={Login} />
    <Route path='/k/:id' component={BoardReader} />
</Layout>;