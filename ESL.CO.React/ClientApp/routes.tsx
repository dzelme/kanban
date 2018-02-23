import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { KanbanBoard } from './components/KanbanBoard';
import { BoardReader } from './components/KanBanReact';

export const routes = <Layout>
    <Route exact path='/' component={BoardList} />
    <Route path='/kanbanboard' component={KanbanBoard} />
    <Route path='/kanbanreact' component={BoardReader} />
</Layout>;